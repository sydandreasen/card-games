using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpTrash : MonoBehaviour
{
    [SerializeField] private GameObject unshuffledDeck;
    public GameObject[] deck = new GameObject[52]; // don't use jokers in trash
    public GameObject[] playerCardBacks = new GameObject[10];
    private static GameObject[] cardBacks;
    public GameObject[] opponentCardBacks = new GameObject[10];
    [SerializeField] private GameObject deckCardBack;
    private List<int> shuffleOrder = new List<int>(52);
    public Vector3[] opponentLocs = new Vector3[10];
    public Vector3[] playerLocs = new Vector3[10];
    [SerializeField] private Transform cardPlaceHolder;
    [SerializeField] private GameObject yourTurn;

    // Start is called before the first frame update
    void Start()
    {
        opponentLocs[0] = new Vector3(200f, 100f, 1);
        opponentLocs[1] = new Vector3(100f, 100f, 1);
        opponentLocs[2] = new Vector3(0, 100f, 1);
        opponentLocs[3] = new Vector3(-100f, 100f, 1);
        opponentLocs[4] = new Vector3(-200f, 100f, 1);
        opponentLocs[5] = new Vector3(200f, 200f, 1);
        opponentLocs[6] = new Vector3(100f, 200f, 1);
        opponentLocs[7] = new Vector3(0, 200f, 1);
        opponentLocs[8] = new Vector3(-100f, 200f, 1);
        opponentLocs[9] = new Vector3(-200f, 200f, 1);

        playerLocs[0] = new Vector3(-200f, -100f, 1);
        playerLocs[1] = new Vector3(-100f, -100f, 1);
        playerLocs[2] = new Vector3(0, -100f, 1);
        playerLocs[3] = new Vector3(100f, -100f, 1);
        playerLocs[4] = new Vector3(200f, -100f, 1);
        playerLocs[5] = new Vector3(-200f, -200f, 1);
        playerLocs[6] = new Vector3(-100f, -200f, 1);
        playerLocs[7] = new Vector3(0, -200f, 1);
        playerLocs[8] = new Vector3(100f, -200f, 1);
        playerLocs[9] = new Vector3(200f, -200f, 1);

        shuffle();
        setStartCards();
        setCardBacks();
        cardBacks = playerCardBacks;
        yourTurn.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // shuffles cards randomly into GameObject[] deck
    public void shuffle()
    {
        System.Random get = new System.Random();
        int newVal;

        //newVal = 0;
        //deck[0] = GameObject.Find("Ace of Clubs");
        //shuffleOrder.Add(newVal);

        for (int i = 0; i < shuffleOrder.Capacity; i++)
        {
            do
            {
                newVal = get.Next(0, 52);
            } while(shuffleOrder.Contains(newVal));
            shuffleOrder.Add(newVal);
            
            switch (newVal)
            {
                case 0:
                    deck[i] = GameObject.Find("Ace of Clubs");
                    break;
                case 1:
                    deck[i] = GameObject.Find("Two of Clubs");
                    break;
                case 2:
                    deck[i] = GameObject.Find("Three of Clubs");
                    break;
                case 3:
                    deck[i] = GameObject.Find("Four of Clubs");
                    break;
                case 4:
                    deck[i] = GameObject.Find("Five of Clubs");
                    break;
                case 5:
                    deck[i] = GameObject.Find("Six of Clubs");
                    break;
                case 6:
                    deck[i] = GameObject.Find("Seven of Clubs");
                    break;
                case 7:
                    deck[i] = GameObject.Find("Eight of Clubs");
                    break;
                case 8:
                    deck[i] = GameObject.Find("Nine of Clubs");
                    break;
                case 9:
                    deck[i] = GameObject.Find("Ten of Clubs");
                    break;
                case 10:
                    deck[i] = GameObject.Find("Jack of Clubs");
                    break;
                case 11:
                    deck[i] = GameObject.Find("Queen of Clubs");
                    break;
                case 12:
                    deck[i] = GameObject.Find("King of Clubs");
                    break;
                case 13:
                    deck[i] = GameObject.Find("Ace of Diamonds");
                    break;
                case 14:
                    deck[i] = GameObject.Find("Two of Diamonds");
                    break;
                case 15:
                    deck[i] = GameObject.Find("Three of Diamonds");
                    break;
                case 16:
                    deck[i] = GameObject.Find("Four of Diamonds");
                    break;
                case 17:
                    deck[i] = GameObject.Find("Five of Diamonds");
                    break;
                case 18:
                    deck[i] = GameObject.Find("Six of Diamonds");
                    break;
                case 19:
                    deck[i] = GameObject.Find("Seven of Diamonds");
                    break;
                case 20:
                    deck[i] = GameObject.Find("Eight of Diamonds");
                    break;
                case 21:
                    deck[i] = GameObject.Find("Nine of Diamonds");
                    break;
                case 22:
                    deck[i] = GameObject.Find("Ten of Diamonds");
                    break;
                case 23:
                    deck[i] = GameObject.Find("Jack of Diamonds");
                    break;
                case 24:
                    deck[i] = GameObject.Find("Queen of Diamonds");
                    break;
                case 25:
                    deck[i] = GameObject.Find("King of Diamonds");
                    break;
                case 26:
                    deck[i] = GameObject.Find("Ace of Hearts");
                    break;
                case 27:
                    deck[i] = GameObject.Find("Two of Hearts");
                    break;
                case 28:
                    deck[i] = GameObject.Find("Three of Hearts");
                    break;
                case 29:
                    deck[i] = GameObject.Find("Four of Hearts");
                    break;
                case 30:
                    deck[i] = GameObject.Find("Five of Hearts");
                    break;
                case 31:
                    deck[i] = GameObject.Find("Six of Hearts");
                    break;
                case 32:
                    deck[i] = GameObject.Find("Seven of Hearts");
                    break;
                case 33:
                    deck[i] = GameObject.Find("Eight of Hearts");
                    break;
                case 34:
                    deck[i] = GameObject.Find("Nine of Hearts");
                    break;
                case 35:
                    deck[i] = GameObject.Find("Ten of Hearts");
                    break;
                case 36:
                    deck[i] = GameObject.Find("Jack of Hearts");
                    break;
                case 37:
                    deck[i] = GameObject.Find("Queen of Hearts");
                    break;
                case 38:
                    deck[i] = GameObject.Find("King of Hearts");
                    break;
                case 39:
                    deck[i] = GameObject.Find("Ace of Spades");
                    break;
                case 40:
                    deck[i] = GameObject.Find("Two of Spades");
                    break;
                case 41:
                    deck[i] = GameObject.Find("Three of Spades");
                    break;
                case 42:
                    deck[i] = GameObject.Find("Four of Spades");
                    break;
                case 43:
                    deck[i] = GameObject.Find("Five of Spades");
                    break;
                case 44:
                    deck[i] = GameObject.Find("Six of Spades");
                    break;
                case 45:
                    deck[i] = GameObject.Find("Seven of Spades");
                    break;
                case 46:
                    deck[i] = GameObject.Find("Eight of Spades");
                    break;
                case 47:
                    deck[i] = GameObject.Find("Nine of Spades");
                    break;
                case 48:
                    deck[i] = GameObject.Find("Ten of Spades");
                    break;
                case 49:
                    deck[i] = GameObject.Find("Jack of Spades");
                    break;
                case 50:
                    deck[i] = GameObject.Find("Queen of Spades");
                    break;
                case 51:
                    deck[i] = GameObject.Find("King of Spades");
                    break;
            }
        }
    }

    // sets starting positions of cards based on shuffle
    public void setStartCards()
    {
        int loc = 0;
        for (int i = 0; i < 20; i += 2)
        {
            deck[i].transform.localPosition = opponentLocs[loc];
            deck[i].GetComponent<Card>().setDrawn(true);
            deck[i + 1].transform.localPosition = playerLocs[loc];
            deck[i + 1].GetComponent<Card>().setDrawn(true);
            loc++;
        }
        deck[20].transform.SetPositionAndRotation(cardPlaceHolder.position, cardPlaceHolder.rotation);
        deck[20].GetComponent<Card>().setActive();
    }

    void setCardBacks()
    {
        deckCardBack.transform.localPosition = new Vector3(0, 0, 1.1f);
        for (int i = 0; i < 10; i++)
        {
            playerCardBacks[i].transform.localPosition = playerLocs[i];
            opponentCardBacks[i].transform.localPosition = opponentLocs[i];
        }
    }

    public GameObject[] getPlayerBacks()
    {
        return playerCardBacks;
    }

    public GameObject[] getOpponentBacks()
    {
        return opponentCardBacks;
    }

    public GameObject[] getDeck()
    {
        return deck;
    }

    public Vector3 getActiveCardPos()
    {
        return cardPlaceHolder.position;
    }

    public Vector3[] getPlayerLocs()
    {
        return playerLocs;
    }

    public void restartGame()
    {
        //Start();
        //deck = new GameObject[52];
        //shuffle();
        //setStartCards();
        //setCardBacks();
        //yourTurn.SetActive(true);
    }
}
