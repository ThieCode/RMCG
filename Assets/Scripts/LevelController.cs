using GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class LevelController : MonoBehaviour
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
    List<Card> generatedCards;
    bool isPaused = false;

    private void GenerateContainer()
    {
        ClearContainer();
        showingCards = new List<Card>();
        var levelLayout = JsonUtility.FromJson<LevelLayout>(selectedLevel.text);

        if (containerParent == null)
            containerParent = this.transform;

        int width = levelLayout.width;
        int height = levelLayout.height;

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

                Vector3 pos = new Vector3(
                    x * cellSize - centerX + containerParent.position.x,
                    0f,
                    y * cellSize - centerZ + containerParent.position.z
                );

                if (isLeft && isBottom) InstantiatePart(cornerPrefab, pos, 180).name = $"Corner_rot{180}";
                if (isRight && isBottom) InstantiatePart(cornerPrefab, pos, 90).name = $"Corner_rot{90}";
                if (isRight && isTop) InstantiatePart(cornerPrefab, pos, 0).name = $"Corner_rot{0}";
                if (isLeft && isTop) InstantiatePart(cornerPrefab, pos, 270).name = $"Corner_rot{270}";

                if (isBottom) InstantiatePart(edgePrefab, pos, 180).name = $"Edge_{x}_{y}";
                if (isTop) InstantiatePart(edgePrefab, pos, 0).name = $"Edge_{x}_{y}";
                if (isLeft) InstantiatePart(edgePrefab, pos, 270).name = $"Edge_{x}_{y}";
                if (isRight) InstantiatePart(edgePrefab, pos, 90).name = $"Edge_{x}_{y}";

                InstantiatePart(tilePrefab, pos, 0).name = $"Tile_{x}_{y}";
            }
        }
    }

    private void GenerateCards()
    {
        var levelLayout = JsonUtility.FromJson<LevelLayout>(selectedLevel.text);

        int width = levelLayout.width;
        int height = levelLayout.height;
        generatedCards = new List<Card>();

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
                    generatedCards.Add(card.GetComponent<Card>());
                }
            }
        }

        float overridenScale = 1 * defaultScaleFactor / (Mathf.Max(width, height) + 1);
        containerParent.localScale = new Vector3(overridenScale, 1, overridenScale);
        PopulateCardsSigns();
    }

    private void PopulateCardsSigns()
    {
        var cardsToPopulate = new List<Card>(generatedCards);
        availableSigns = new List<Sprite>();
        availableSigns.AddRange(gameSigns);
        int requiredSignsAmount = cardsToPopulate.Count / 2;
        int signID = 0;
        for (int i = 0; i < requiredSignsAmount; i++)
        {
            var signIndex = UnityEngine.Random.Range(0, availableSigns.Count);
            var cardIndex = UnityEngine.Random.Range(0, cardsToPopulate.Count);
            cardsToPopulate[cardIndex].SetSign(availableSigns[signIndex], signID);
            cardsToPopulate.RemoveAt(cardIndex);
            cardIndex = UnityEngine.Random.Range(0, cardsToPopulate.Count);
            cardsToPopulate[cardIndex].SetSign(availableSigns[signIndex], signID);
            cardsToPopulate.RemoveAt(cardIndex);
            availableSigns.RemoveAt(signIndex);
            signID++;
        }
    }

    private GameObject InstantiatePart(GameObject partPrefab, Vector3 pos, float yRot)
    {
        var part = Instantiate(partPrefab, pos, Quaternion.identity, containerParent);
        part.transform.localRotation *= Quaternion.Euler(-90, yRot, 0);
        return part;
    }

    private void OnCardPressed(CardClickedEvent @event)
    {
        if (isPaused) return;
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
            GameEventBus.Raise(new CardFlippedEvent(@event.Card));
            showingCards.Add(@event.Card);

            if(showingCards.Count == 2)
            {
                var cardA = showingCards[0];
                var cardB = showingCards[1];

                if(cardA.EqualsInSprite(cardB))
                {
                    GameEventBus.Raise(new CardsMatchedEvent(cardA, cardB));
                    showingCards.Clear();
                    generatedCards.Remove(cardA);
                    generatedCards.Remove(cardB);
                    if (generatedCards.Count == 0) GameEventBus.Raise(new LevelSolvedEvent());
                }
            }
        }
    }

    private void OnEnable()
    {
        GameEventBus.Subscribe<LevelSelectEvent>(OnLevelSelected);
        GameEventBus.Subscribe<CardClickedEvent>(OnCardPressed);
        GameEventBus.Subscribe<PauseMenuOpenEvent>(OnPauseMenuOpened);
        GameEventBus.Subscribe<PauseMenuCloseEvent>(OnPauseMenuClosed);
    }

    private void OnPauseMenuOpened(PauseMenuOpenEvent @event)
    {
        isPaused = true;
    }

    private void OnPauseMenuClosed(PauseMenuCloseEvent @event)
    {
        isPaused = false;
    }

    private void OnLevelSelected(LevelSelectEvent @event)
    {
        selectedLevel = @event.LevelLayoutJson;
        GenerateContainer();
        GenerateCards();
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<CardClickedEvent>(OnCardPressed);
        GameEventBus.Unsubscribe<LevelSelectEvent>(OnLevelSelected);
    }

    public void ClearContainer()
    {
        containerParent.localScale = Vector3.one;
        if (containerParent == null)
            containerParent = this.transform;

        for (int i = containerParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(containerParent.GetChild(i).gameObject);

    }
}

public struct LevelLayout
{
    public int width, height;
    public bool[] hasACard;
}
