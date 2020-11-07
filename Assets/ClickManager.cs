using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickManager : MonoBehaviour
{

    // game setup to access the deck, card backs for player and opponent
    [SerializeField] private SetUpTrash game;

    // prompt "your turn"
    [SerializeField] private GameObject prompt;

    // initialize deck to null
    private GameObject[] deck = null;

    // timer to delay computer play after user has completed their turn
    private float timerTotal = 2.0f;

    // the active, counting down, amount of time in delay between user turn end and computer turn start
    private float timerActive;

    // manage wether the delay between user turn end and computer turn start is an active timer
    private bool timerOn;

    // manage whether first call to computer turn has been made
    private bool firstCompCall = false;

    // the card to carry through progession of the game to mark how far into shuffled deck the game is
    // used as a reference point to find the next card needed when drawing a new card is necessary
    private GameObject carryCard;

    // card actively in play
    private GameObject activeCard;

    // prompt that player won or lost
    [SerializeField] private GameObject playerLost;
    [SerializeField] private GameObject playerWon;

    // delay intialization of player's turn
    private float delayTurn;
    private bool delayOn = false;
    private float delayLength = 2.5f;
    
    void Start()
    {
        playerLost.SetActive(false);
        playerWon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (carryCard == null)
        {
            deck = game.getDeck();
            carryCard = deck[20];
            activeCard = deck[20];
        }

        delayInitializeComputerTurn();

        delayInitializePlayerTurn();

        checkGameOver();
    }

    // triggered when the active card is clicked, or when
    // a spot is chosen for a king to be placed
    public void play(Card clicked)
    {
        // activeCard = king, user chose place to put it
        bool kingActive = clicked.getSuit().Contains("Back");
        // if the card in play next to deck is not a face card
        bool notFaceCard = clicked.getValue() <= 10;

        // if king active
        if (kingActive)
        {
            // get number value chosen.
            // value comes from the card back,
            // so the placeholder number
            int value = clicked.getValue();

            // deactive the card back from the game, visually
            clicked.gameObject.SetActive(false);

            // find the underneath card
            activeCard = deck[(value * 2) - 1]; // *2 reflects alternation in players/opponents cards from shuffled deck

            // find the king next to deck
            GameObject king = getKing(game.getActiveCardPos().x, game.getActiveCardPos().y);

            // switch the cards, card2 is one to next be in play
            switchCards(king, game.getActiveCardPos(), activeCard, activeCard.transform.position, true);

            // deactive the buttons on the card backs, don't allow picking anymore locations to place a king
            GameObject[] backs = game.getPlayerBacks();
            for (int i = 0; i < 10; i++)
            {
                backs[i].GetComponent<UnityEngine.UI.Button>().enabled = false;
            }
        }
        // if the card in play next to deck is not a face card
        else if (notFaceCard)
        {
            // in order to check if a king is there, which could be reput into play
            Vector3 loc = game.getPlayerBacks()[clicked.gameObject.GetComponent<Card>().getValue() - 1].transform.position;
            float x = loc.x;
            float y = loc.y;

            // the card back for the number matching the card in play is active
            // (then the player may play, turn not over)
            bool numNotYetPlayed = game.getPlayerBacks()[clicked.getValue() - 1].activeSelf;
            
            if (numNotYetPlayed)
            {
                // find back of card on player's side that matches the value of the active card and remove that card back to reveal the card underneath
                game.getPlayerBacks()[clicked.getValue() - 1].SetActive(false);

                // find the card that was just uncovered and is now active in play
                activeCard = deck[(clicked.getValue() * 2) - 1]; // the underneath card

                switchCards(clicked.gameObject, game.getActiveCardPos(), activeCard, activeCard.transform.position, true);

                // note that increasing the fixed timestep in Edit>Project Settings>Time makes the movement animation more visible
                // also note rigid bodies must be kinematically enabled to show that animation
            }
            else if (isKingAvailable(x, y, loc)) // there is a king in the spot with the card's value that can be brought into play
            {
                // switch the cards out and reactive the button on the king, and deactivate the button on the nonFaceCard
                GameObject king = getKing(x, y);
                switchCards(activeCard, game.getActiveCardPos(), king, loc, true);
                activeCard = king;
            }
            else
            {
                // turn over
                prompt.GetComponent<UnityEngine.UI.Text>().text = "You already have that spot filled! Your turn is over";

                // start timer before drawing a new card on top of the trash/turn-ending card
                timerOn = true;
                timerActive = timerTotal;
            }
        }
        // king in play, must allow player to pick a spot to play the card
        else if (clicked.getValue() == 13)
        {
            // king, pick where to play
            prompt.GetComponent<UnityEngine.UI.Text>().text = "Wild card! Pick a place below to use it!";

            // activate the buttons on the card backs
            GameObject[] backs = game.getPlayerBacks();
            for (int i = 0; i < 10; i++)
            {
                backs[i].GetComponent<UnityEngine.UI.Button>().enabled = true;
            }
        }
        else // jack or queen in play
        {
            // turn over
            prompt.GetComponent<UnityEngine.UI.Text>().text = "That's Trash! Your Turn is Over.";

            // start timer before drawing a new card on top of the trash/turn-ending card
            timerOn = true;
            timerActive = timerTotal;
        }
    }

    // the automated play for the computer
    private void computerTurn(bool firstCall) // first call means only check if top is trash first time handing over from player
    {
        if (firstCall)
        {
            prompt.GetComponent<UnityEngine.UI.Text>().text = "";

            // first check if a king is there, which could be reput into play
            bool kingAvailable = false; // initialize

            if (activeCard.GetComponent<Card>().getValue() <= 10)
            {
                Vector3 loc = game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].transform.position;
                float x = loc.x;
                float y = loc.y;

                kingAvailable = isKingAvailable(x, y, loc);
            }

            // check top card to see if it's junk jack/queen/card already have or if usable
            bool cardIsPlayable = (activeCard.GetComponent<Card>().getValue() <= 10
                && game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf) ||
                (kingAvailable && !game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf);

            if (cardIsPlayable)
            {
                // don't draw new card, use current active card
                // don't do anything. progress 
                computerTurn(false);
            }
            // need to pull a new card
            else
            {
                // else, pull a new card
                GameObject newCard = drawNextCard(false);
                
                timerOn = true;
                timerActive = timerTotal;
            }
        }
        else
        {
            // allow continuous computer play while the active card in play is face card or king
            bool faceOrKing = activeCard.GetComponent<Card>().getValue() <= 10 || activeCard.GetComponent<Card>().getValue() == 13;
            // while (faceOrKing)
            if (faceOrKing)
            {
                // if face card active 
                bool faceCard = activeCard.GetComponent<Card>().getValue() <= 10;
                bool kingInPlay = activeCard.GetComponent<Card>().getValue() == 13;
                if (faceCard)
                {
                    // number has not been played
                    bool notYetPlayed = game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf;

                    // first check if a king is there, which could be reput into play
                    Vector3 loc = game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].transform.position;
                    float x = loc.x;
                    float y = loc.y;
                    
                    if (notYetPlayed)
                    {
                        // find back of card on opponent's side that matches the value of the active card and remove that card back to reveal the card underneath
                        game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].SetActive(false);

                        // find the revealed card
                        GameObject newCard = deck[(activeCard.GetComponent<Card>().getValue() * 2) - 2]; // the underneath card, 2s b/c of alternating in dealing shuffled deck

                        switchCards(activeCard, game.getActiveCardPos(), newCard, newCard.transform.position, false);

                        // note that increasing the fixed timestep in Edit>Project Settings>Time makes the movement animation more visible
                        // also note rigid bodies must be kinematically enabled to show that animation

                        // switch to new activeCard, play on that card should be determined the next time through while loop
                        activeCard = newCard;
                    }
                    else if (isKingAvailable(x, y, loc))
                    {
                        // switch the cards out and deactivate the buttons
                        GameObject king = getKing(x, y);
                        switchCards(activeCard, game.getActiveCardPos(), king, loc, false);
                        activeCard = king;
                    }
                }
                else if (kingInPlay)
                {
                    // hold onto the king that's in play 
                    GameObject king = activeCard;

                    // king, pick where to play
                    int value = 0;
                    bool numAlreadyChosen = false;
                    do
                    {
                        value = Random.Range(1, 11);
                        numAlreadyChosen = !game.getOpponentBacks()[value - 1].activeSelf; // whether card back still active at the randomly selected value or not
                    } while (numAlreadyChosen);

                    // deactive tht card back to reveal the card below
                    game.getOpponentBacks()[value - 1].SetActive(false);

                    // find the card underneath
                    activeCard = deck[(value * 2) - 2]; // 2's are because of alternation between player and opponent in dealing cards\]

                    switchCards(king, game.getActiveCardPos(), activeCard, activeCard.transform.position, false);
                }
                else
                {
                    // turn over
                    delayTurn = delayLength;
                    delayOn = true;
                }
            }
            else
            {
                delayTurn = delayLength;
                delayOn = true;
            }

            // determine whether additional calls to computer turn should occur, or if it's the player's turn
            bool kingAvailableEndTurn = false;
            if (activeCard.GetComponent<Card>().getValue() <= 10)
            {
                Vector3 loc = game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].transform.position;
                float x = loc.x;
                float y = loc.y;
                kingAvailableEndTurn = isKingAvailable(x, y, loc);
            }
            bool stillCompTurn = activeCard.GetComponent<Card>().getValue() == 13 ||
                                (activeCard.GetComponent<Card>().getValue() <= 10 &&
                                game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf) ||
                                (kingAvailableEndTurn && !game.getOpponentBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf);
            if (stillCompTurn)
            {
                timerOn = true;
                timerActive = timerTotal;
            }
            else
            {
                delayTurn = delayLength;
                delayOn = true;
            }
        }
    }

    // set up to allow the player to play
    void initializePlayerTurn()
    {
        firstCompCall = false; // reset calls to computerTurn

        // first check if a king is there, which could be reput into play
        bool kingAvailable = false;
        if(activeCard.GetComponent<Card>().getValue() <= 10)
        {
            // first check if a king is there, which could be reput into play
            Vector3 loc = game.getPlayerBacks()[activeCard.GetComponent<Card>().getValue() - 1].transform.position;
            float x = loc.x;
            float y = loc.y;

            kingAvailable = isKingAvailable(x, y, loc);
        }

        // turn over. back to player
        // check to see if new card should be drawn or if can use handed over card
        bool needNewCard = (activeCard.GetComponent<Card>().getValue() == 11 || // jack is trash
                            activeCard.GetComponent<Card>().getValue() == 12 || // queen is trash
                            !game.getPlayerBacks()[activeCard.GetComponent<Card>().getValue() - 1].activeSelf) && // the card back has been removed from that value slot; that number has already been played
                            !kingAvailable; // replay a king
        if (needNewCard)
        {
            // draw new card
            GameObject newCard = drawNextCard(true);
        }
        else // use passed card from computer's turn
        {
            activeCard.GetComponent<UnityEngine.UI.Button>().enabled = true; // make the card clickable so that player can click
        }
        prompt.GetComponent<UnityEngine.UI.Text>().text = "Your Turn Again! Click the active card to play.";
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Trash");
    }

    GameObject getKing(float x, float y)
    {
        // find the king in play
        GameObject[] kings = new GameObject[4];
        int kingCount = 0; // for breaking out of for loop asap

        // loop through deck to find kings
        for (int i = 0; i < deck.Length; i++)
        {
            // use tag to find if current card is a king
            if (deck[i].CompareTag("Kings"))
            {
                // add king from deck into array with just king
                kings[kingCount] = deck[i];
                // increment to next king
                kingCount++;
                if (kingCount == 4)
                {
                    // if found all kings, break out of loop
                    break;
                }
            }
        }

        // find the correct king
        GameObject king = null;
        // loop through kings array
        for (int i = 0; i < 4; i++)
        {
            // find the king that exists in the same position that the
            // active card in play sits (next to deck)
            bool inActivePos = Mathf.Abs(kings[i].transform.position.x - x) < 0.05 && Mathf.Abs(kings[i].transform.position.y - y) < 0.05 ;
            if (inActivePos)
            {
                // save that certain king
                king = kings[i];
            }
        }
        return king;
    }

    void delayInitializePlayerTurn()
    {
        // run the timer between computer turn and player's turn
        if (delayOn && delayTurn > 0.0f)
        {
            delayTurn -= Time.deltaTime;
        }
        if (delayOn && delayTurn <= 0.0f)
        {
            delayOn = false;
            initializePlayerTurn();
        }
    }

    void delayInitializeComputerTurn()
    {
        // run the timer between end of user turn
        // and beginning of computer turn
        if (timerActive > 0.0f && timerOn)
        {
            timerActive -= Time.deltaTime;
        }
        if (timerActive <= 0.0f && timerOn)
        {
            timerOn = false;
            // turn timer off and start computer turn
            // computerTurn(true);
            if (!firstCompCall)
            {
                firstCompCall = true;
                computerTurn(true);
            }
            else
            {
                computerTurn(false);
            }
        }
    }

    void checkGameOver()
    {
        // check for game over at any point
        bool playerSpotsLeft = false; // default to false, if any are present, switch to true and game still active
        bool opponentSpotsLeft = false; // default to false, if any are present, switch to true and game still active
        for (int i = 0; i < 10; i++)
        {
            if (game.getPlayerBacks()[i].activeSelf)
            {
                playerSpotsLeft = true;
            }
            if (game.getOpponentBacks()[i].activeSelf)
            {
                opponentSpotsLeft = true;
            }
        }
        // now check if either got flagged true, if not, end game
        // by showing prompt and deactiving the button on current card
        if (!playerSpotsLeft)
        {
            playerWon.SetActive(true);
            activeCard.GetComponent<UnityEngine.UI.Button>().enabled = false;
            prompt.GetComponent<UnityEngine.UI.Text>().text = "";
        }
        if (!opponentSpotsLeft)
        {
            playerLost.SetActive(true);
            activeCard.GetComponent<UnityEngine.UI.Button>().enabled = false;
            prompt.GetComponent<UnityEngine.UI.Text>().text = "";
        }
    }

    void switchCards(GameObject card1, Vector3 loc1, GameObject card2, Vector3 loc2, bool card2buttonOn)
    {
        // switch positions
        card1.GetComponent<Rigidbody>().MovePosition(loc2);
        card2.GetComponent<Rigidbody>().MovePosition(loc1);

        // enable/disable buttons accordingly
        card1.GetComponent<UnityEngine.UI.Button>().enabled = false;
        card2.GetComponent<UnityEngine.UI.Button>().enabled = card2buttonOn;
    }

    bool isKingAvailable(float x, float y, Vector3 loc)
    {
        // find kings and if any of them are at loc
        bool kingAvailable = false;
        int kingCount = 0; // for breaking out of for loop asap
        for (int i = 0; i < deck.Length; i++)
        {
            // use tag to find if current card is a king
            if (deck[i].CompareTag("Kings"))
            {
                // check if king at relevant location
                if (Mathf.Abs(deck[i].transform.position.x - x) < 0.05 && Mathf.Abs(deck[i].transform.position.y - y) < 0.05)
                {
                    kingAvailable = true;
                    break;
                }
                // increment to next king
                kingCount++;
                if (kingCount == 4)
                {
                    // if found all kings, break out of loop
                    break;
                }
            }
        }
        return kingAvailable;
    }

    GameObject drawNextCard(bool buttonOn)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            // find the card in the deck that matches the last pulled card
            bool lastDrawn = deck[i].GetComponent<Card>().getName() == carryCard.GetComponent<Card>().getName() && i < deck.Length - 1;
            if (lastDrawn && i < deck.Length - 1) // i cannot be last in deck since need to access i + 1 for newCard
            {
                // deactivate card on top of stack and then grab the new
                activeCard.SetActive(false);
                GameObject newCard = deck[i + 1];
                carryCard = newCard; // save for next turn switch
                activeCard = newCard; // save for moving cards around
                newCard.GetComponent<Rigidbody>().MovePosition(game.getActiveCardPos()); // move the drawn card to the active card spot next to the deck
                newCard.GetComponent<UnityEngine.UI.Button>().enabled = buttonOn; // whether player is allowed to play on that card
                return newCard; // the newCard to draw from the deck is the card in the deck after the lastDrawn
            }
            else if (i == deck.Length - 1)
            {
                // game should be over
            }
        }
        // if haven't found card, return null
        return null;
    }
}
