using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorsLike.EditorTools
{
    public static class SkillSelectionViewBuilder
    {
        private const string PREFAB_PATH = "Assets/_Project/UI/InGame/SkillSelectionView.prefab";

        [MenuItem("SurvivorsLike/UI/Build SkillSelectionView Prefab")]
        public static void Build()
        {
            // ── Canvas_SkillSelection ──────────────────────────────────────
            var canvasGO = new GameObject("Canvas_SkillSelection");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // ── SkillSelectionView ─────────────────────────────────────────
            var viewGO = CreateUI("SkillSelectionView", canvasGO.transform);
            SetStretch(RT(viewGO));

            // ── DimOverlay ─────────────────────────────────────────────────
            var dimGO = CreateUI("DimOverlay", viewGO.transform);
            SetStretch(RT(dimGO));
            var dimImg = dimGO.AddComponent<Image>();
            dimImg.color = new Color(0f, 0f, 0f, 0.75f);
            dimImg.raycastTarget = true;

            // ── Panel ──────────────────────────────────────────────────────
            var panelGO = CreateUI("Panel", viewGO.transform);
            var panelRT = RT(panelGO);
            SetCenter(panelRT, 960f, 700f);

            var panelImg = panelGO.AddComponent<Image>();
            panelImg.color = new Color(0.08f, 0.09f, 0.13f, 0.97f);
            panelImg.raycastTarget = true;

            var panelVLG = panelGO.AddComponent<VerticalLayoutGroup>();
            panelVLG.childAlignment = TextAnchor.UpperCenter;
            panelVLG.spacing = 20f;
            panelVLG.padding = new RectOffset(30, 30, 30, 30);
            panelVLG.childControlWidth = false;
            panelVLG.childControlHeight = false;
            panelVLG.childForceExpandWidth = false;
            panelVLG.childForceExpandHeight = false;

            // ── HeaderGroup ────────────────────────────────────────────────
            var headerGO = CreateUI("HeaderGroup", panelGO.transform);
            RT(headerGO).sizeDelta = new Vector2(900f, 120f);

            var headerVLG = headerGO.AddComponent<VerticalLayoutGroup>();
            headerVLG.childAlignment = TextAnchor.UpperCenter;
            headerVLG.spacing = 8f;
            headerVLG.childControlWidth = false;
            headerVLG.childControlHeight = false;
            headerVLG.childForceExpandWidth = false;
            headerVLG.childForceExpandHeight = false;

            // TitleLabel
            var titleGO = CreateUI("TitleLabel", headerGO.transform);
            RT(titleGO).sizeDelta = new Vector2(900f, 64f);
            var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "레벨 업!";
            titleTMP.fontSize = 52f;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
            titleTMP.raycastTarget = false;

            // SubtitleLabel
            var subGO = CreateUI("SubtitleLabel", headerGO.transform);
            RT(subGO).sizeDelta = new Vector2(900f, 40f);
            var subTMP = subGO.AddComponent<TextMeshProUGUI>();
            subTMP.text = "스킬을 선택하세요";
            subTMP.fontSize = 28f;
            subTMP.alignment = TextAlignmentOptions.Center;
            subTMP.color = new Color(0.78f, 0.78f, 0.78f, 1f);
            subTMP.raycastTarget = false;

            // ── CardContainer ──────────────────────────────────────────────
            var containerGO = CreateUI("CardContainer", panelGO.transform);
            RT(containerGO).sizeDelta = new Vector2(900f, 490f);

            var hlg = containerGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 20f;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            // ── SkillCardView × 3 ─────────────────────────────────────────
            BuildSkillCard(containerGO.transform, 0);
            BuildSkillCard(containerGO.transform, 1);
            BuildSkillCard(containerGO.transform, 2);

            // ── 저장 ───────────────────────────────────────────────────────
            string dir = Path.GetDirectoryName(PREFAB_PATH);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                canvasGO, PREFAB_PATH, InteractionMode.AutomatedAction, out bool success);

            if (success)
                Debug.Log($"[SkillSelectionViewBuilder] 프리팹 저장 완료: {PREFAB_PATH}");
            else
                Debug.LogError("[SkillSelectionViewBuilder] 프리팹 저장 실패!");

            Object.DestroyImmediate(canvasGO);
            AssetDatabase.Refresh();
        }

        // ──────────────────────────────────────────────────────────────────
        //  카드 한 장 빌드
        // ──────────────────────────────────────────────────────────────────
        private static void BuildSkillCard(Transform parent, int index)
        {
            var cardGO = CreateUI($"SkillCardView_{index}", parent);
            RT(cardGO).sizeDelta = new Vector2(280f, 470f);

            var cardImg = cardGO.AddComponent<Image>();
            cardImg.color = new Color(0.12f, 0.14f, 0.19f, 1f);

            var btn = cardGO.AddComponent<Button>();
            var btnColors = ColorBlock.defaultColorBlock;
            btnColors.normalColor  = new Color(0.12f, 0.14f, 0.19f, 1f);
            btnColors.highlightedColor = new Color(0.18f, 0.35f, 0.70f, 1f);
            btnColors.pressedColor = new Color(0.10f, 0.25f, 0.55f, 1f);
            btnColors.selectedColor = new Color(0.15f, 0.30f, 0.60f, 1f);
            btn.colors = btnColors;
            btn.targetGraphic = cardImg;

            var cardVLG = cardGO.AddComponent<VerticalLayoutGroup>();
            cardVLG.childAlignment = TextAnchor.UpperCenter;
            cardVLG.spacing = 12f;
            cardVLG.padding = new RectOffset(16, 16, 24, 24);
            cardVLG.childControlWidth = false;
            cardVLG.childControlHeight = false;
            cardVLG.childForceExpandWidth = false;
            cardVLG.childForceExpandHeight = false;

            // IconImage
            var iconGO = CreateUI("IconImage", cardGO.transform);
            RT(iconGO).sizeDelta = new Vector2(96f, 96f);
            var iconImg = iconGO.AddComponent<Image>();
            iconImg.color = new Color(0.28f, 0.30f, 0.40f, 1f);
            iconImg.preserveAspect = true;
            iconImg.raycastTarget = false;

            // SkillNameLabel
            var nameGO = CreateUI("SkillNameLabel", cardGO.transform);
            RT(nameGO).sizeDelta = new Vector2(248f, 42f);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = "스킬 이름";
            nameTMP.fontSize = 22f;
            nameTMP.fontStyle = FontStyles.Bold;
            nameTMP.alignment = TextAlignmentOptions.Center;
            nameTMP.color = Color.white;
            nameTMP.raycastTarget = false;

            // Divider
            var divGO = CreateUI("Divider", cardGO.transform);
            RT(divGO).sizeDelta = new Vector2(248f, 2f);
            var divImg = divGO.AddComponent<Image>();
            divImg.color = new Color(0.38f, 0.40f, 0.50f, 1f);
            divImg.raycastTarget = false;

            // DescriptionLabel
            var descGO = CreateUI("DescriptionLabel", cardGO.transform);
            RT(descGO).sizeDelta = new Vector2(248f, 170f);
            var descTMP = descGO.AddComponent<TextMeshProUGUI>();
            descTMP.text = "스킬 설명이 여기에\n표시됩니다.";
            descTMP.fontSize = 18f;
            descTMP.alignment = TextAlignmentOptions.Center;
            descTMP.color = new Color(0.72f, 0.72f, 0.72f, 1f);
            descTMP.enableWordWrapping = true;
            descTMP.raycastTarget = false;

            // LevelBadge
            var badgeGO = CreateUI("LevelBadge", cardGO.transform);
            RT(badgeGO).sizeDelta = new Vector2(248f, 46f);
            var badgeImg = badgeGO.AddComponent<Image>();
            badgeImg.color = index == 1
                ? new Color(0.10f, 0.50f, 0.25f, 1f)   // 신규: 초록
                : new Color(0.12f, 0.28f, 0.65f, 1f);  // 업그레이드: 파랑
            badgeImg.raycastTarget = false;

            var badgeHLG = badgeGO.AddComponent<HorizontalLayoutGroup>();
            badgeHLG.childAlignment = TextAnchor.MiddleCenter;
            badgeHLG.childControlWidth = false;
            badgeHLG.childControlHeight = false;
            badgeHLG.childForceExpandWidth = false;
            badgeHLG.childForceExpandHeight = false;

            var badgeLabelGO = CreateUI("BadgeLabel", badgeGO.transform);
            RT(badgeLabelGO).sizeDelta = new Vector2(220f, 40f);
            var badgeTMP = badgeLabelGO.AddComponent<TextMeshProUGUI>();
            badgeTMP.text = index == 1 ? "NEW!" : "Lv 1  →  2";
            badgeTMP.fontSize = 20f;
            badgeTMP.fontStyle = FontStyles.Bold;
            badgeTMP.alignment = TextAlignmentOptions.Center;
            badgeTMP.color = Color.white;
            badgeTMP.raycastTarget = false;
        }

        // ──────────────────────────────────────────────────────────────────
        //  헬퍼
        // ──────────────────────────────────────────────────────────────────
        private static GameObject CreateUI(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            return go;
        }

        private static RectTransform RT(GameObject go) =>
            go.GetComponent<RectTransform>();

        private static void SetStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        private static void SetCenter(RectTransform rt, float w, float h)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(w, h);
            rt.anchoredPosition = Vector2.zero;
        }
    }
}
