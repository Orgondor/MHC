using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject playerAreaPrefab;
    public int numPlayers = 4;
    public int startingHealth = 40;
    public int startingCommanderDamage = 0;

    private List<PlayAreaUI> playerAreas;
    private bool clickBegan = true;

    private GameObject options;
    private RectTransform openOptions;
    private int[] oValues = { 1, -1, 5, -5 };
    private TextMeshProUGUI oHealth;
    private List<RectTransform> oHealthPlusMinus;
    private TextMeshProUGUI oCommanderDamage;
    private List<RectTransform> oCommanderDamagePlusMinus;
    private Toggle oCommanderDamageToggle;
    private TextMeshProUGUI oPlayers;
    private List<RectTransform> oPlayersPlusMinus;
    private RectTransform oNewGame;
    private RectTransform oBack;

    // Start is called before the first frame update
    void Start()
    {
        // Setup the options page
        options = GameObject.Find("Options");
        options.SetActive(false);
        openOptions = GameObject.Find("OpenOptions").GetComponent<RectTransform>();

        // Health
        oHealth = options.transform.Find("Health").GetComponent<TextMeshProUGUI>();
        oHealth.text = string.Format("{0}", startingHealth);
        oHealthPlusMinus = new List<RectTransform>();
        oHealthPlusMinus.Add(oHealth.transform.Find("+1").GetComponent<RectTransform>());
        oHealthPlusMinus.Add(oHealth.transform.Find("-1").GetComponent<RectTransform>());
        oHealthPlusMinus.Add(oHealth.transform.Find("+5").GetComponent<RectTransform>());
        oHealthPlusMinus.Add(oHealth.transform.Find("-5").GetComponent<RectTransform>());

        // Commander Damage
        oCommanderDamage = options.transform.Find("CommanderDamage").GetComponent<TextMeshProUGUI>();
        oCommanderDamage.text = string.Format("{0}", startingCommanderDamage);
        oCommanderDamagePlusMinus = new List<RectTransform>();
        oCommanderDamagePlusMinus.Add(oCommanderDamage.transform.Find("+1").gameObject.GetComponent<RectTransform>());
        oCommanderDamagePlusMinus.Add(oCommanderDamage.transform.Find("-1").gameObject.GetComponent<RectTransform>());
        oCommanderDamagePlusMinus.Add(oCommanderDamage.transform.Find("+5").gameObject.GetComponent<RectTransform>());
        oCommanderDamagePlusMinus.Add(oCommanderDamage.transform.Find("-5").gameObject.GetComponent<RectTransform>());
        oCommanderDamageToggle = oCommanderDamage.transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        oCommanderDamageToggle.onValueChanged.AddListener(delegate
        {
            ToggleCommanderDamage(oCommanderDamageToggle);
        });

        // Players
        oPlayers = options.transform.Find("Players").GetComponent<TextMeshProUGUI>();
        oPlayers.text = string.Format("{0}", numPlayers);
        oPlayersPlusMinus = new List<RectTransform>();
        oPlayersPlusMinus.Add(oPlayers.transform.Find("+1").gameObject.GetComponent<RectTransform>());
        oPlayersPlusMinus.Add(oPlayers.transform.Find("-1").gameObject.GetComponent<RectTransform>());

        // New Game Button
        oNewGame = options.transform.Find("NewGame").GetComponent<RectTransform>();

        // Back Button
        oBack = options.transform.Find("Back").GetComponent<RectTransform>();

        playerAreas = new List<PlayAreaUI>();
        CreatePlayerAreas();
    }

    void ResetPlayerAreas()
    {
        foreach (PlayAreaUI playArea in playerAreas)
        {
            playArea.SetValues(startingHealth, startingCommanderDamage);
        }
    }

    void CreatePlayerAreas()
    {
        // Rearange previous areas
        foreach (PlayAreaUI playArea in playerAreas)
        {
            playArea.SetNumPlayers(numPlayers);
            playArea.SetupArea();
        }

        // Create new areas
        for (int i = playerAreas.Count; i < numPlayers; i++)
        {
            GameObject playerArea = Instantiate(playerAreaPrefab);
            PlayAreaUI ui = playerArea.GetComponent<PlayAreaUI>();
            playerAreas.Add(ui);
            ui.SetPlayer(i + 1);
            ui.SetNumPlayers(numPlayers);
            ui.SetValues(startingHealth, startingCommanderDamage);
            ui.FirstSetup();

            if (options.activeSelf)
                ui.gameObject.SetActive(false);
        }
    }

    void DestroyPlayerAreas()
    {
        for (int i = playerAreas.Count - 1; i > numPlayers - 1; i--)
        {
            Debug.LogFormat("playerAreas.Count: {0}, i: {1}", playerAreas.Count, i);
            PlayAreaUI playerArea = playerAreas[i];
            Destroy(playerArea.gameObject);
            playerAreas.RemoveAt(i);
        }

        // Rearange remaning areas
        foreach (PlayAreaUI playArea in playerAreas)
        {
            playArea.SetNumPlayers(numPlayers);
            playArea.SetupArea();
        }
    }

    void ToggleCommanderDamage(Toggle toggle)
    {

    }

    void ToggleOptions(bool on)
    {
        foreach (PlayAreaUI playerArea in playerAreas)
        {
            playerArea.gameObject.SetActive(!on);
        }
        openOptions.gameObject.SetActive(!on);

        options.SetActive(on);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && clickBegan)
        {
            Vector2 pos = Input.mousePosition;

            if (options.activeSelf)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(oNewGame, pos))
                {
                    ResetPlayerAreas();
                    ToggleOptions(false);
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(oBack, pos))
                {
                    ToggleOptions(false);
                }
                else
                {
                    int i = 0;
                    foreach (RectTransform rect in oHealthPlusMinus)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
                        {
                            startingHealth += oValues[i];
                            oHealth.text = string.Format("{0}", startingHealth);
                            return;
                        }
                        i++;
                    }

                    i = 0;
                    foreach (RectTransform rect in oCommanderDamagePlusMinus)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
                        {
                            startingCommanderDamage += oValues[i];
                            oCommanderDamage.text = string.Format("{0}", startingCommanderDamage);
                            return;
                        }
                        i++;
                    }

                    i = 0;
                    foreach (RectTransform rect in oPlayersPlusMinus)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(rect, pos))
                        {
                            numPlayers += oValues[i];
                            oPlayers.text = string.Format("{0}", numPlayers);
                            if (oValues[i] > 0)
                                CreatePlayerAreas();
                            else
                                DestroyPlayerAreas();
                            return;
                        }
                        i++;
                    }
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(openOptions, pos))
                {
                    ToggleOptions(true);
                }
                else
                {
                    foreach (PlayAreaUI playerArea in playerAreas)
                    {
                        if (playerArea.ProcessClick(pos))
                            return;
                    }
                }
            }
        }
        else if (!Input.GetMouseButtonDown(0))
            clickBegan = true;
    }
}
