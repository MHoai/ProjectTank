using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        private int m_NumRoundsToWin = 1;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;
        public GameObject m_BoxPrefab;
        public GameObject m_TowerPrefab;
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks

        public GameObject m_MobNormal0;
        public GameObject m_MobNormal1;
        public GameObject m_MobNormal2;
        public MobManager[] m_Mobs;
        public BoxManager[] m_Boxs;
        public TowerManager[] m_Towers;

        public Button AI_button;
        public Button NetworkBtn;
        public Button HostBtn;
        public Button ClientBtn;

        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

        private bool m_WaitingMenu = true;
        private bool set_up_AI_once = true;

        public GameObject m_TankPrefabNet;
        public TankManager[] tanksNetwork;

        public GameObject LevelArt;

        private void Start()
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);

            // Once the tanks have been created and the camera is using them as targets, start the game.
            //StartCoroutine(GameLoop());
        }

        #region AI Mode
        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }

            for (int i = 0; i < m_Mobs.Length; i++)//
            {
                if (m_Mobs[i].m_TypeMob == 0)
                {
                    m_Mobs[i].m_Instance =
                        Instantiate(m_MobNormal0, m_Mobs[i].m_SpawnPoint.position, m_Mobs[i].m_SpawnPoint.rotation) as GameObject;
                }

                else if (m_Mobs[i].m_TypeMob == 1)
                {
                    m_Mobs[i].m_Instance =
                        Instantiate(m_MobNormal1, m_Mobs[i].m_SpawnPoint.position, m_Mobs[i].m_SpawnPoint.rotation) as GameObject;
                }

                else if (m_Mobs[i].m_TypeMob == 2)
                {
                    m_Mobs[i].m_Instance =
                        Instantiate(m_MobNormal2, m_Mobs[i].m_SpawnPoint.position, m_Mobs[i].m_SpawnPoint.rotation) as GameObject;
                }

                m_Mobs[i].m_PlayerNumber = i + 1;
                m_Mobs[i].Setup();
            }
            // For all the boxs...
            for (int i = 0; i < m_Boxs.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                m_Boxs[i].m_Instance =
                    Instantiate(m_BoxPrefab, m_Boxs[i].m_SpawnPoint.position, m_Boxs[i].m_SpawnPoint.rotation) as GameObject;
                m_Boxs[i].Setup();
            }
            // For all the tower...
            for (int i = 0; i < m_Boxs.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                m_Towers[i].m_Instance =
                    Instantiate(m_TowerPrefab, m_Towers[i].m_SpawnPoint.position, m_Towers[i].m_SpawnPoint.rotation) as GameObject;
                m_Towers[i].m_PlayerNumber = i + 1;
                m_Towers[i].Setup();

            }

        }


        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_Tanks.Length + m_Mobs.Length];

            // For each of these transforms...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            for (int i = m_Tanks.Length; i < m_Tanks.Length + m_Mobs.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Mobs[i - m_Tanks.Length].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }

        public void AI_button_pressed()
        {

            AI_button.gameObject.SetActive(false);
            NetworkBtn.gameObject.SetActive(false);
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);
            StartCoroutine(GameLoop());
            m_WaitingMenu = false;
            m_NumRoundsToWin = 1;
        }

        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            while (m_WaitingMenu)
            {
                yield return null;
            }

            if (set_up_AI_once == true && !m_WaitingMenu)
            {
                set_up_AI_once = false;
                SpawnAllTanks();
                SetCameraTargets();
            }
            if (!m_WaitingMenu)
            {
                // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundStarting());

                // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
                yield return StartCoroutine(RoundPlaying());

                // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
                yield return StartCoroutine(RoundEnding());

                // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
                //if (m_GameWinner != null) du thang hay thua cung chi co 1 round
                //{
                // If there is a game winner, restart the level.
                SceneManager.LoadScene(0);
                //}
                //else
                //{
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                //   StartCoroutine(GameLoop());
                // }
            }
        }

        private IEnumerator RoundStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks();
            DisableTankControl();

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            m_CameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_MessageText.text = "START";// "ROUND " + m_RoundNumber;

            for (int i = 0; i < m_Mobs.Length; i++)
            {
                m_Mobs[i].m_StateManager.PlayerLocation = m_Tanks[0].m_Instance.transform;
            }

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }

        private IEnumerator RoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl();

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!NoneTankLeft() && !NoneMobLeft())
            {
                for (int i = 0; i < m_Mobs.Length; i++)
                {
                    m_Mobs[i].m_StateManager.PlayerLocation = m_Tanks[0].m_Instance.transform;
                    m_Mobs[i].m_StateManager.oneTankLeft = false;
                }

                // ... return on the next frame.
                yield return null;
            }
            for (int i = 0; i < m_Mobs.Length; i++)
            {
                m_Mobs[i].m_StateManager.oneTankLeft = true;
            }
        }


        private IEnumerator RoundEnding()
        {
            // Stop tanks from moving.
            DisableTankControl();

            // Clear the winner from the previous round.
            m_RoundWinner = null;

            // See if there is a winner now the round is over.
            m_RoundWinner = GetRoundWinner();

            // If there is a winner, increment their score.
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game.
            m_GameWinner = GetGameWinner();

            // Get a message based on the scores and whether or not there is a game winner and display it.
            string message = EndMessage();
            m_MessageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool NoneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 0;
        }

        private bool NoneMobLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            //Get tank addition form mob
            int addTank = 0;

            // Go through all the tanks...
            for (int i = 0; i < m_Mobs.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Mobs[i].m_Instance.activeSelf)
                    numTanksLeft++;
                addTank += m_Mobs[i].GetAddTankNumber();
            }
            numTanksLeft += addTank;

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 0;
        }

        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "Game Over!";

            // If there is a winner then change the message to reflect that.
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that.
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }

            for (int i = 0; i < m_Mobs.Length; i++)
            {
                m_Mobs[i].Reset();
            }
            for (int i = 0; i < m_Boxs.Length; i++)
            {
                m_Boxs[i].Reset();
            }
            for (int i = 0; i < m_Towers.Length; i++)
            {
                m_Towers[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }

            for (int i = 0; i < m_Mobs.Length; i++)
            {
                m_Mobs[i].EnableControl();
            }
            for (int i = 0; i < m_Boxs.Length; i++)
            {
                m_Boxs[i].EnableControl();
            }
            for (int i = 0; i < m_Towers.Length; i++)
            {
                m_Towers[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }

            for (int i = 0; i < m_Mobs.Length; i++)
            {
                m_Mobs[i].DisableControl();
            }
            for (int i = 0; i < m_Boxs.Length; i++)
            {
                m_Boxs[i].DisableControl();
            }
            for (int i = 0; i < m_Towers.Length; i++)
            {
                m_Towers[i].DisableControl();
            }
        }

        #endregion

        #region NetWork
        public void NetworkBtnPressed()
        {
            AI_button.gameObject.SetActive(false);
            NetworkBtn.gameObject.SetActive(false);
            HostBtn.gameObject.SetActive(true);
            ClientBtn.gameObject.SetActive(true);
        }

        public void HostBtnPressed()
        {
            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);
            NetworkManager.Singleton.StartHost();
            //StartCoroutine(NetworkGameLoop());
            m_MessageText.text = "";
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[LevelArt.transform.childCount];

            // For each of these transforms...
            for (int i = 0; i < LevelArt.transform.childCount; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = LevelArt.transform.GetChild(i);
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }

        public void ClientBtnPressed()
        {
            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);
            NetworkManager.Singleton.StartClient();
            //StartCoroutine(NetworkGameLoop());
            m_MessageText.text = "";
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[LevelArt.transform.childCount];

            // For each of these transforms...
            for (int i = 0; i < LevelArt.transform.childCount; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = LevelArt.transform.GetChild(i);
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }

        private IEnumerator NetworkGameLoop()
        {
            m_MessageText.text = "";
               // Create a collection of transforms the same size as the number of tanks.
               Transform[] targets = new Transform[LevelArt.transform.childCount];

            // For each of these transforms...
            for (int i = 0; i < LevelArt.transform.childCount; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = LevelArt.transform.GetChild(i);
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
            yield return null;
            ////while (m_WaitingMenu)
            ////{
            ////    yield return null;
            ////}

            ////if (set_up_AI_once == true && !m_WaitingMenu)
            ////{
            ////    set_up_AI_once = false;
            ////    SpawnAllTanks();
            //for (int i = 0; i < tanksNetwork.Length; i++)
            //{
            //    // ... create them, set their player number and references needed for control.
            //    tanksNetwork[i].m_Instance =
            //        Instantiate(m_TankPrefabNet, tanksNetwork[i].m_SpawnPoint.position, tanksNetwork[i].m_SpawnPoint.rotation) as GameObject;
            //    tanksNetwork[i].m_PlayerNumber = i + 1;
            //    tanksNetwork[i].Setup();
            //}
            ////SetCameraTargets();

            //// Create a collection of transforms the same size as the number of tanks.
            //Transform[] targets = new Transform[tanksNetwork.Length];

            //// For each of these transforms...
            //for (int i = 0; i < tanksNetwork.Length; i++)
            //{
            //    // ... set it to the appropriate tank transform.
            //    targets[i] = tanksNetwork[i].m_Instance.transform;
            //}

            //// These are the targets the camera should follow.
            //m_CameraControl.m_Targets = targets;

            ////}
            ////if (!m_WaitingMenu)
            ////{
            //// Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            //yield return StartCoroutine(RoundStartingNet());

            //// Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            //yield return StartCoroutine(RoundPlayingNet());

            //// Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            //yield return StartCoroutine(RoundEndingNet());

            //// This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            ////if (m_GameWinner != null) du thang hay thua cung chi co 1 round
            ////{
            //// If there is a game winner, restart the level.
            //SceneManager.LoadScene(0);
            ////}
            ////else
            ////{
            //// If there isn't a winner yet, restart this coroutine so the loop continues.
            //// Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            ////   StartCoroutine(GameLoop());
            //// }
            ////}
        }

        private IEnumerator RoundStartingNet()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            for (int i = 0; i < tanksNetwork.Length; i++)
            {
                tanksNetwork[i].Reset();
            }
            for (int i = 0; i < tanksNetwork.Length; i++)
            {
                tanksNetwork[i].DisableControl();
            }

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            m_CameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_MessageText.text = "START";// "ROUND " + m_RoundNumber;

            //for (int i = 0; i < m_Mobs.Length; i++)
            //{
            //    m_Mobs[i].m_StateManager.PlayerLocation = m_Tanks[0].m_Instance.transform;
            //}

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }

        private IEnumerator RoundPlayingNet()
        {
            // As soon as the round begins playing let the players control the tanks.
            for (int i = 0; i < tanksNetwork.Length; i++)
            {
                tanksNetwork[i].EnableControl();
            }

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!NoneTankLeftNet())
            {
                //for (int i = 0; i < m_Mobs.Length; i++)
                //{
                //    m_Mobs[i].m_StateManager.PlayerLocation = m_Tanks[0].m_Instance.transform;
                //    m_Mobs[i].m_StateManager.oneTankLeft = false;
                //}

                // ... return on the next frame.
                yield return null;
            }
            //for (int i = 0; i < m_Mobs.Length; i++)
            //{
            //    m_Mobs[i].m_StateManager.oneTankLeft = true;
            //}
        }

        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool NoneTankLeftNet()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            for (int i = 0; i < tanksNetwork.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (tanksNetwork[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 0;
        }


        private IEnumerator RoundEndingNet()
        {
            // Stop tanks from moving.
            for (int i = 0; i < tanksNetwork.Length; i++)
            {
                tanksNetwork[i].DisableControl();
            }


            //// Clear the winner from the previous round.
            //m_RoundWinner = null;

            //// See if there is a winner now the round is over.
            //m_RoundWinner = GetRoundWinner();

            //// If there is a winner, increment their score.
            //if (m_RoundWinner != null)
            //    m_RoundWinner.m_Wins++;

            //// Now the winner's score has been incremented, see if someone has one the game.
            //m_GameWinner = GetGameWinner();

            //// Get a message based on the scores and whether or not there is a game winner and display it.
            //string message = EndMessage();
            m_MessageText.text = "GAME OVER!!!";

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }

    }
}
#endregion
