using GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LevelContainerGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    private float cellSize = 1f;
    private float defaultScaleFactor = 4;
    [SerializeField] private float cardYDisplacement = 0.15f;
    [SerializeField] private Sprite[] gameSigns;
    [Header("Prefabs")]
    [SerializeField] private GameObject cornerPrefab;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject cardPrefab;

    [Header("Parent Object (optional)")]
    [SerializeField] private Transform containerParent;

    [SerializeField] private TextAsset selectedLevel;
    private List<Sprite> availableSigns = new List<Sprite>();
    [SerializeField] List<Card> showingCards = new List<Card>();

    private void GenerateContainer()
    {
        ClearContainer();
        showingCards = new List<Card>();
        var levelLayout = JsonUtility.FromJson<LevelLayout>(selectedLevel.text);

        if (containerParent == null)
            containerParent = this.transform;

        int width = levelLayout.width;
        int height = levelLayout.height;

        // --- CENTERING ---
        // Grid indices go from 0..width-1 and 0..height-1
        // Center in world space is halfway between min and max indices.
        float centerX = (width - 1) * 0.5f * cellSize;
        float centerZ = (height - 1) * 0.5f * cellSize;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isLeft = (x == 0);
                bool isRight = (x == width - 1);
                bool isBottom = (y == 0);
                bool isTop = (y == height - 1);

                // Centered position
                Vector3 pos = new Vector3(
                    x * cellSize - centerX + containerParent.position.x,
                    0f,
                    y * cellSize - centerZ + containerParent.position.z
                );

                // 1. CORNERS
                if (isLeft && isBottom) InstantiatePart(cornerPrefab, pos, 180).name = $"Corner_rot{180}";
                if (isRight && isBottom) InstantiatePart(cornerPrefab, pos, 90).name = $"Corner_rot{90}";
                if (isRight && isTop) InstantiatePart(cornerPrefab, pos, 0).name = $"Corner_rot{0}";
                if (isLeft && isTop) InstantiatePart(cornerPrefab, pos, 270).name = $"Corner_rot{270}";

                // 2. EDGES
                if (isBottom) InstantiatePart(edgePrefab, pos, 180).name = $"Edge_{x}_{y}";
                if (isTop) InstantiatePart(edgePrefab, pos, 0).name = $"Edge_{x}_{y}";
                if (isLeft) InstantiatePart(edgePrefab, pos, 270).name = $"Edge_{x}_{y}";
                if (isRight) InstantiatePart(edgePrefab, pos, 90).name = $"Edge_{x}_{y}";

                // 3. INNER TILE (always)
                InstantiatePart(tilePrefab, pos, 0).name = $"Tile_{x}_{y}";
            }
        }
    }

    private void GenerateCards()
    {
        var levelLayout = JsonUtility.FromJson<LevelLayout>(selectedLevel.text);

        int width = levelLayout.width;
        int height = levelLayout.height;
        List<Card> cardsToPopulate = new List<Card>();
        // Same centering as container
        float centerX = (width-1) * 0.5f * cellSize;
        float centerZ = (height-1) * 0.5f * cellSize;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;

                if (levelLayout.hasACard[index])
                {
                    Vector3 pos = new Vector3(
                        x * cellSize - centerX + containerParent.position.x,
                        0f,
                        y * cellSize - centerZ + containerParent.position.z
                    );

                    var card = InstantiatePart(cardPrefab, pos, 0);
                    card.name = $"Card_{x}_{y}";
                    card.transform.localPosition += Vector3.up * cardYDisplacement;
                    cardsToPopulate.Add(card.GetComponent<Card>());
                }
            }
        }

        float overridenScale = 1 * defaultScaleFactor / (Mathf.Max(width, height) + 1);
        containerParent.localScale = new Vector3(overridenScale, 1, overridenScale);
        PopulateCardsSigns(cardsToPopulate);
    }

    private void PopulateCardsSigns(List<Card> cardsToPopulate)
    {
        availableSigns = new List<Sprite>();
        availableSigns.AddRange(gameSigns);
        int requiredSignsAmount = cardsToPopulate.Count / 2;
        for (int i = 0; i < requiredSignsAmount; i++)
        {
            var signIndex = UnityEngine.Random.Range(0, availableSigns.Count);
            var cardIndex = UnityEngine.Random.Range(0, cardsToPopulate.Count);
            cardsToPopulate[cardIndex].SetSign(availableSigns[signIndex]);
            cardsToPopulate.RemoveAt(cardIndex);
            cardIndex = UnityEngine.Random.Range(0, cardsToPopulate.Count);
            cardsToPopulate[cardIndex].SetSign(availableSigns[signIndex]);
            cardsToPopulate.RemoveAt(cardIndex);
            availableSigns.RemoveAt(signIndex);
        }
    }

    private GameObject InstantiatePart(GameObject partPrefab, Vector3 pos, float yRot)
    {
        var part = Instantiate(partPrefab, pos, Quaternion.identity, containerParent);
        // Your parts seem to be modeled laying down, so keep the -90 on X,
        // and just vary Y by the given rotation.
        part.transform.localRotation *= Quaternion.Euler(-90, yRot, 0);
        return part;
    }

    private Sprite GetRandomAvailableSign()
    {
        var index = UnityEngine.Random.Range(0, availableSigns.Count);
        var sprite = availableSigns[index];
        availableSigns.RemoveAt(index);
        return sprite;
    }

    private void OnCardPressed(CardClickedEvent @event)
    {
        if (showingCards.Count > 0 && showingCards.Contains(@event.Card)) return;

        if (showingCards.Count == 2)
        {
            foreach (var card in showingCards)
            {
                card.Hide();
            }
            showingCards.Clear();
        }

        if (showingCards.Count <= 1)
        {
            @event.Card.Show();
            showingCards.Add(@event.Card);
        }
    }

    private void OnEnable()
    {
        GameEventBus.Subscribe<CardClickedEvent>(OnCardPressed);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<CardClickedEvent>(OnCardPressed);
    }

    #region Debug
    [ContextMenu("Clear Container")]
    public void ClearContainer()
    {
        containerParent.localScale = Vector3.one;
        if (containerParent == null)
            containerParent = this.transform;

        for (int i = containerParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(containerParent.GetChild(i).gameObject);

    }

    [ContextMenu("Generate Level")]
    public void GenerateLevelTest()
    {
        GenerateContainer();
        GenerateCards();

    }
    #endregion
}

public struct LevelLayout
{
    public int width, height;
    public bool[] hasACard;
}
