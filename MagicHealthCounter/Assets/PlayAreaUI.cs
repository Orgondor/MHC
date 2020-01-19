using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayAreaUI : MonoBehaviour
{
    public TextMeshProUGUI playerHealthUI;
    public TextMeshProUGUI commanderDamageUI;
    public Image centerBackground;
    public float centerFraction = 0.6f;
    public float bottomFraction = 0.2f;

    private int player = 0;
    private int numPlayers = 0;
    private RectTransform rect;
    private int areaWidth;
    private int areaHeight;
    private int playerHealth = 40;
    private int commanderDamage = 0;

    public void FirstSetup()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        rect = GetComponent<RectTransform>();
        SetupArea();

        // Only needed on first setup
        if ((player & 1) != 0)
        {
            Image img = GetComponent<Image>();
            Color tmp = img.color;
            tmp.a *= 0.5f;
            img.color = tmp;
        }
    }

    public void SetupArea()
    {
        RectTransform cbgrt = centerBackground.GetComponent<RectTransform>();

        int layoutWidth = Mathf.CeilToInt(numPlayers / 2.0f);
        areaWidth = Screen.width / layoutWidth;
        areaHeight = Screen.height / 2;
        int layoutX = (player - 1) % layoutWidth + 1;
        int layoutY = player > layoutWidth ? 1 : 0;
        if (layoutY == 1) // invert x order on the top
            layoutX = layoutWidth + 1 - layoutX;

        rect.sizeDelta = new Vector2(areaWidth, areaHeight);
        cbgrt.sizeDelta = new Vector2(areaWidth * centerFraction, areaHeight);
        rect.SetPositionAndRotation(new Vector3(areaWidth * (layoutX - 0.5f), areaHeight * (layoutY + 0.5f), 0.0f), Quaternion.Euler(0, 0, layoutY * 180));
    }

    public void SetValues(int health, int commanderDmg)
    {
        playerHealth = health;
        commanderDamage = commanderDmg;

        playerHealthUI.text = string.Format("{0}", playerHealth);
        commanderDamageUI.text = string.Format("{0}", commanderDamage);
    }

    public void SetPlayer(int playerId)
    {
        player = playerId;
    }

    public void SetNumPlayers(int numberOfPlayers)
    {
        numPlayers = numberOfPlayers;
    }

    public bool ProcessClick(Vector2 screenPos)
    {
        bool hit = RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos);

        if (hit)
        {
            Vector3 localPos = rect.InverseTransformPoint(screenPos.x, screenPos.y, 0.0f);
            int amount = Mathf.RoundToInt((Mathf.Abs(localPos.x) > areaWidth * centerFraction * 0.5f ? 5 : 1) * Mathf.Sign(localPos.x));

            if (localPos.y > -bottomFraction * areaHeight)
            {
                playerHealth += amount;
                playerHealthUI.text = string.Format("{0}", playerHealth);
            }
            else
            {
                commanderDamage += amount;
                commanderDamageUI.text = string.Format("{0}", commanderDamage);
            }
        }

        return hit;
    }
}
