using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToEnemy : MonoBehaviour
{
    private RectTransform m_transform;
    private Camera m_camera;
    public Canvas canvas;
    public Image healthBar;
    public TMP_Text characterName;

    private void Awake()
    {
        m_transform = transform.GetComponent<RectTransform>();
        m_camera = Camera.main;
    }

    public void SetEnemyInfo(BattleCharacter SelectedEnemy)
    {
        characterName.text = SelectedEnemy.nameCharacter;
        UIHelper.HealthBarPercent(SelectedEnemy.HP, SelectedEnemy.maxHP);
    }

    public void ChangePosition(Vector3 target)
    {
        m_transform.anchoredPosition = GetUIPosFromWorldPos(canvas, target);
        if(healthBar == null)
        {
            return;
        }
    }

    public Vector2 GetUIPosFromWorldPos(Canvas canvas, Vector3 worldPosition)
    {
        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, m_camera, out canvasPos);

        return canvasPos;
    }
}
