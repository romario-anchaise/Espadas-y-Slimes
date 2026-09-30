using System;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

[InitializeOnLoad]
public static class Semana6UIUXSetup
{
    const string SetupKey = "AnimatorS5.Semana6UIUXSetup.v4";
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
    const string OldLevelPath = "Assets/Scenes/SampleScene.unity";
    const string LevelPath = "Assets/Scenes/Level_01.unity";

    static Semana6UIUXSetup()
    {
        EditorApplication.delayCall += RunOnce;
    }

    [MenuItem("Tools/Semana 6/Configurar UI UX")]
    public static void ConfigureFromMenu()
    {
        ConfigureProject();
        EditorPrefs.SetBool(SetupKey, true);
    }

    static void RunOnce()
    {
        if (EditorPrefs.GetBool(SetupKey, false) || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        try
        {
            ConfigureProject();
            EditorPrefs.SetBool(SetupKey, true);
            Debug.Log("[Semana 6] UI/UX configurada: HUD, pausa, feedback, puntos y escenas.");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    static void ConfigureProject()
    {
        RenameAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        ConfigureBuildScenes();
        ConfigureLevel();
        ConfigureMainMenu();

        EditorSceneManager.OpenScene(LevelPath, OpenSceneMode.Single);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void RenameAssets()
    {
        if (AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/scripts/DanioPinchos.cs") == null &&
            AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/scripts/pinchos.cs") != null)
        {
            string error = AssetDatabase.MoveAsset("Assets/scripts/pinchos.cs", "Assets/scripts/DanioPinchos.cs");
            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException(error);
        }

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LevelPath) == null &&
            AssetDatabase.LoadAssetAtPath<SceneAsset>(OldLevelPath) != null)
        {
            string error = AssetDatabase.MoveAsset(OldLevelPath, LevelPath);
            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException(error);
        }
    }

    static void ConfigureLevel()
    {
        Scene scene = EditorSceneManager.OpenScene(LevelPath, OpenSceneMode.Single);
        GameObject canvasObject = FindRootOrChild(scene, "Canvas");
        if (canvasObject == null)
            throw new InvalidOperationException("No se encontró el Canvas del nivel.");

        ConfigureCanvas(canvasObject);

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        RectTransform hud = GetOrCreateUIObject(canvasRect, "HUD");
        Transform oldHud = FindDeep(canvasObject.transform, "HUB");
        if (oldHud != null && oldHud != hud)
        {
            while (oldHud.childCount > 0)
                oldHud.GetChild(0).SetParent(hud, false);
            UnityEngine.Object.DestroyImmediate(oldHud.gameObject);
        }
        ConfigureHudRect(hud);

        Image barraFondo = GetOrCreateImage(hud, "BarraFondo");
        ConfigureTopLeft(barraFondo.rectTransform, new Vector2(48f, -45f), new Vector2(420f, 48f));
        barraFondo.color = new Color(0.08f, 0.06f, 0.12f, 0.92f);

        Image barraVida = GetOrCreateImage(hud, "BarraVida");
        barraVida.transform.SetParent(barraFondo.transform, false);
        Stretch(barraVida.rectTransform, 5f);
        barraVida.type = Image.Type.Filled;
        barraVida.fillMethod = Image.FillMethod.Horizontal;
        barraVida.fillOrigin = (int)Image.OriginHorizontal.Left;
        barraVida.fillAmount = 1f;
        barraVida.color = new Color(0.76f, 0.08f, 0.13f, 1f);

        TMP_Text textoPuntos = GetOrCreateText(hud, "TextoPuntos");
        ConfigureTopRight(textoPuntos.rectTransform, new Vector2(-48f, -34f), new Vector2(360f, 72f));
        textoPuntos.text = "0000";
        textoPuntos.alignment = TextAlignmentOptions.TopRight;
        textoPuntos.enableAutoSizing = true;
        textoPuntos.fontSizeMin = 24f;
        textoPuntos.fontSizeMax = 48f;
        textoPuntos.color = Color.white;

        TMP_Text avisoPuntos = GetOrCreateText(canvasRect, "AvisoPuntos");
        avisoPuntos.rectTransform.anchorMin = avisoPuntos.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        avisoPuntos.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        avisoPuntos.rectTransform.sizeDelta = new Vector2(420f, 100f);
        avisoPuntos.rectTransform.anchoredPosition = new Vector2(0f, -40f);
        avisoPuntos.alignment = TextAlignmentOptions.Center;
        avisoPuntos.font = textoPuntos.font;
        avisoPuntos.fontSize = 56f;
        avisoPuntos.text = string.Empty;
        avisoPuntos.color = new Color(1f, 0.82f, 0.2f, 0f);
        avisoPuntos.raycastTarget = false;

        Image overlayDanio = GetOrCreateImage(canvasRect, "OverlayDanio");
        Stretch(overlayDanio.rectTransform, 0f);
        overlayDanio.color = new Color(1f, 0f, 0f, 0f);
        overlayDanio.raycastTarget = false;
        overlayDanio.transform.SetAsLastSibling();

        GameObject pauseObject = FindDeep(canvasObject.transform, "PAUSAPANEL")?.gameObject ??
                                 FindDeep(canvasObject.transform, "PausaPanel")?.gameObject;
        if (pauseObject == null)
            pauseObject = GetOrCreateUIObject(canvasRect, "PausaPanel").gameObject;
        pauseObject.name = "PausaPanel";
        RectTransform pauseRect = pauseObject.GetComponent<RectTransform>();
        Stretch(pauseRect, 0f);
        Image pauseBackground = pauseObject.GetComponent<Image>() ?? pauseObject.AddComponent<Image>();
        pauseBackground.color = new Color(0.03f, 0.02f, 0.08f, 0.84f);
        pauseBackground.raycastTarget = true;

        UIManager uiManager = UnityEngine.Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
        if (uiManager == null)
        {
            GameObject managerObject = new GameObject("UIManager");
            uiManager = managerObject.AddComponent<UIManager>();
        }
        SetReference(uiManager, "barraVida", barraVida);
        SetReference(uiManager, "textoPuntos", textoPuntos);
        SetReference(uiManager, "overlayDanio", overlayDanio);
        SetReference(uiManager, "avisoPuntos", avisoPuntos);

        PauseMenu pauseMenu = uiManager.GetComponent<PauseMenu>() ?? uiManager.gameObject.AddComponent<PauseMenu>();
        GameObject resumeButton = FindDeep(pauseObject.transform, "REANUDAR")?.gameObject;
        if (resumeButton == null)
            resumeButton = FindDeep(pauseObject.transform, "Reanudar")?.gameObject;
        GameObject menuButton = FindDeep(pauseObject.transform, "SALIR AL MENU")?.gameObject;
        SetReference(pauseMenu, "panelPausa", pauseObject);
        SetReference(pauseMenu, "primerBoton", resumeButton);
        EnsureButtonListener(resumeButton, pauseMenu, nameof(PauseMenu.Reanudar));
        EnsureButtonListener(menuButton, pauseMenu, nameof(PauseMenu.IrAlMenu));

        PlayerHealth playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>(FindObjectsInactive.Include);
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.AddComponent<PlayerHealth>();
        }

        GameObject spikes = FindRootOrChild(scene, "spikes");
        if (spikes != null)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(spikes);
            if (spikes.GetComponent<DanioPinchos>() == null)
                spikes.AddComponent<DanioPinchos>();
        }

        EnsurePointPickups(scene);
        pauseObject.SetActive(false);

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        GameObject prefabCopy = UnityEngine.Object.Instantiate(pauseObject);
        prefabCopy.name = "PausaPanel";
        foreach (Button button in prefabCopy.GetComponentsInChildren<Button>(true))
            button.onClick = new Button.ButtonClickedEvent();
        PrefabUtility.SaveAsPrefabAsset(prefabCopy, "Assets/Prefabs/PausaPanel.prefab");
        UnityEngine.Object.DestroyImmediate(prefabCopy);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void ConfigureMainMenu()
    {
        Scene scene = EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);
        GameObject canvasObject = FindRootOrChild(scene, "Canvas");
        if (canvasObject != null)
        {
            ConfigureCanvas(canvasObject);
            StyleMainMenu(canvasObject);
        }

        GameObject playButton = FindRootOrChild(scene, "Jugar");
        MainMenu menu = UnityEngine.Object.FindFirstObjectByType<MainMenu>(FindObjectsInactive.Include);
        EnsureButtonListener(playButton, menu, nameof(MainMenu.Jugar));
        EnsureButtonListener(FindRootOrChild(scene, "Salir"), menu, nameof(MainMenu.Salir));

        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (eventSystem != null && playButton != null)
            eventSystem.firstSelectedGameObject = playButton;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void StyleMainMenu(GameObject canvasObject)
    {
        RectTransform canvas = canvasObject.GetComponent<RectTransform>();
        Image sky = GetOrCreateImage(canvas, "FondoCielo");
        Image mountains = GetOrCreateImage(canvas, "FondoMontanas");
        Image forest = GetOrCreateImage(canvas, "FondoBosque");

        sky.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/2D Forest Tileset Pack Toon Style/Night/Background Night/NIght sky.png");
        mountains.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/2D Forest Tileset Pack Toon Style/Night/Background Night/Night Mountains Far.png");
        forest.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/2D Forest Tileset Pack Toon Style/Night/Background Night/Night Trees Close.png");

        foreach (Image background in new[] { sky, mountains, forest })
        {
            background.color = Color.white;
            background.preserveAspect = false;
            background.raycastTarget = false;
        }
        Stretch(sky.rectTransform, 0f);
        ConfigureBottomLayer(mountains.rectTransform, 640f);
        ConfigureBottomLayer(forest.rectTransform, 560f);
        sky.transform.SetAsFirstSibling();
        mountains.transform.SetSiblingIndex(1);
        forest.transform.SetSiblingIndex(2);

        Image panel = GetOrCreateImage(canvas, "PanelMenu");
        RectTransform panelRect = panel.rectTransform;
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(650f, 680f);
        panel.color = new Color(0.025f, 0.055f, 0.12f, 0.88f);
        Outline panelOutline = panel.GetComponent<Outline>() ?? panel.gameObject.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.15f, 0.7f, 0.78f, 0.75f);
        panelOutline.effectDistance = new Vector2(4f, -4f);

        TMP_FontAsset menuFont = GetOrCreateCrispMenuFont();
        TMP_Text title = null;
        foreach (TMP_Text text in canvasObject.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.text.Replace("\n", " ").Trim().Contains("Espadas y Slimes"))
            {
                title = text;
                break;
            }
        }

        if (title != null)
        {
            title.name = "TituloJuego";
            title.transform.SetParent(panel.transform, false);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 175f);
            titleRect.sizeDelta = new Vector2(560f, 170f);
            title.text = "ESPADAS\nY SLIMES";
            title.alignment = TextAlignmentOptions.Center;
            title.enableAutoSizing = false;
            title.fontSize = 74f;
            title.color = new Color(0.95f, 0.87f, 0.54f, 1f);
            if (menuFont != null)
                title.font = menuFont;
            title.outlineWidth = 0.08f;
            title.outlineColor = new Color32(8, 20, 42, 255);
            title.extraPadding = true;
        }

        GameObject playObject = FindDeep(canvas, "Jugar")?.gameObject;
        GameObject exitObject = FindDeep(canvas, "Salir")?.gameObject;
        StyleMenuButton(playObject, panel.transform, new Vector2(0f, -75f),
            new Color(0.05f, 0.58f, 0.52f, 1f), "JUGAR", menuFont);
        StyleMenuButton(exitObject, panel.transform, new Vector2(0f, -190f),
            new Color(0.56f, 0.12f, 0.2f, 1f), "SALIR", menuFont);

        TMP_Text subtitle = GetOrCreateText(panel.transform, "Subtitulo");
        subtitle.rectTransform.anchorMin = subtitle.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        subtitle.rectTransform.sizeDelta = new Vector2(520f, 55f);
        subtitle.rectTransform.anchoredPosition = new Vector2(0f, 65f);
        subtitle.text = "UNA AVENTURA EN EL BOSQUE NOCTURNO";
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.enableAutoSizing = false;
        subtitle.fontSize = 23f;
        subtitle.color = new Color(0.7f, 0.9f, 0.94f, 1f);
        if (menuFont != null)
            subtitle.font = menuFont;

        TMP_Text controls = GetOrCreateText(panel.transform, "Controles");
        controls.rectTransform.anchorMin = controls.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        controls.rectTransform.sizeDelta = new Vector2(550f, 65f);
        controls.rectTransform.anchoredPosition = new Vector2(0f, -285f);
        controls.text = "A / D  o  FLECHAS  ·  ESPACIO PARA SALTAR";
        controls.alignment = TextAlignmentOptions.Center;
        controls.enableAutoSizing = false;
        controls.fontSize = 20f;
        controls.color = new Color(0.72f, 0.78f, 0.86f, 1f);
        if (menuFont != null)
            controls.font = menuFont;

        panel.transform.SetAsLastSibling();
    }

    static void StyleMenuButton(GameObject buttonObject, Transform parent, Vector2 position,
        Color normalColor, string label, TMP_FontAsset font)
    {
        if (buttonObject == null)
            return;

        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(390f, 88f);

        Image image = buttonObject.GetComponent<Image>();
        if (image != null)
            image.color = normalColor;

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = Color.Lerp(normalColor, Color.white, 0.22f);
            colors.selectedColor = colors.highlightedColor;
            colors.pressedColor = Color.Lerp(normalColor, Color.black, 0.25f);
            colors.disabledColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.45f);
            colors.colorMultiplier = 1f;
            button.colors = colors;
        }

        TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>(true);
        if (buttonText != null)
        {
            buttonText.text = label;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.enableAutoSizing = false;
            buttonText.fontSize = 34f;
            buttonText.color = Color.white;
            if (font != null)
                buttonText.font = font;
            buttonText.outlineWidth = 0.06f;
            buttonText.outlineColor = new Color32(5, 14, 30, 255);
            buttonText.extraPadding = true;
        }
    }

    static TMP_FontAsset GetOrCreateCrispMenuFont()
    {
        const string assetPath = "Assets/FONTS/BlackOpsOne-Crisp SDF.asset";
        TMP_FontAsset existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
        if (existing != null)
            return existing;

        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/FONTS/BlackOpsOne-Regular.ttf");
        if (sourceFont == null)
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/FONTS/BlackOpsOne-Regular SDF.asset");

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            90,
            12,
            GlyphRenderMode.SDFAA_HINTED,
            2048,
            2048,
            AtlasPopulationMode.Dynamic,
            true);
        fontAsset.name = "BlackOpsOne-Crisp SDF";
        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
        fontAsset.isMultiAtlasTexturesEnabled = true;
        AssetDatabase.CreateAsset(fontAsset, assetPath);
        if (fontAsset.material != null)
        {
            fontAsset.material.name = "BlackOpsOne-Crisp Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }
        if (fontAsset.atlasTexture != null)
        {
            fontAsset.atlasTexture.name = "BlackOpsOne-Crisp Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
        }
        EditorUtility.SetDirty(fontAsset);
        AssetDatabase.SaveAssets();
        return fontAsset;
    }

    static void ConfigureBottomLayer(RectTransform rect, float height)
    {
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, height);
    }

    static void ConfigureBuildScenes()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MainMenuPath, true),
            new EditorBuildSettingsScene(LevelPath, true)
        };

        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuPath);
    }

    static void ConfigureCanvas(GameObject canvasObject)
    {
        Canvas canvas = canvasObject.GetComponent<Canvas>() ?? canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>() ?? canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        if (canvasObject.GetComponent<GraphicRaycaster>() == null)
            canvasObject.AddComponent<GraphicRaycaster>();
    }

    static void ConfigureHudRect(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, 160f);
    }

    static void ConfigureTopLeft(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    static void ConfigureTopRight(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    static void Stretch(RectTransform rect, float inset)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = new Vector2(inset, inset);
        rect.offsetMax = new Vector2(-inset, -inset);
    }

    static RectTransform GetOrCreateUIObject(Transform parent, string name)
    {
        Transform found = FindDeep(parent, name);
        if (found != null)
            return found.GetComponent<RectTransform>() ?? found.gameObject.AddComponent<RectTransform>();

        GameObject created = new GameObject(name, typeof(RectTransform));
        created.layer = LayerMask.NameToLayer("UI");
        created.transform.SetParent(parent, false);
        return created.GetComponent<RectTransform>();
    }

    static Image GetOrCreateImage(Transform parent, string name)
    {
        RectTransform rect = GetOrCreateUIObject(parent, name);
        if (rect.parent != parent)
            rect.SetParent(parent, false);
        return rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
    }

    static TMP_Text GetOrCreateText(Transform parent, string name)
    {
        RectTransform rect = GetOrCreateUIObject(parent, name);
        if (rect.parent != parent)
            rect.SetParent(parent, false);
        return rect.GetComponent<TMP_Text>() ?? rect.gameObject.AddComponent<TextMeshProUGUI>();
    }

    static void EnsurePointPickups(Scene scene)
    {
        GameObject parent = FindRootOrChild(scene, "Puntos") ?? new GameObject("Puntos");
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/2D Platformer/Assets/coin/coin(1).png");
        Vector3[] positions =
        {
            new Vector3(-6.1f, -1.8f, 0f),
            new Vector3(-0.8f, -1.4f, 0f),
            new Vector3(4.3f, -1.2f, 0f)
        };

        for (int index = 0; index < positions.Length; index++)
        {
            string name = $"Punto_{index + 1:00}";
            Transform existing = FindDeep(parent.transform, name);
            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            GameObject pickup = new GameObject(name, typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(PointPickup));
            pickup.transform.SetParent(parent.transform, true);
            pickup.transform.position = positions[index];
            pickup.transform.localScale = Vector3.one * 0.75f;

            SpriteRenderer renderer = pickup.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 15;
            CircleCollider2D collider = pickup.GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.45f;
        }
    }

    static void EnsureButtonListener(GameObject buttonObject, MonoBehaviour target, string methodName)
    {
        if (buttonObject == null || target == null)
            return;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
            return;

        for (int index = 0; index < button.onClick.GetPersistentEventCount(); index++)
        {
            if (button.onClick.GetPersistentTarget(index) == target &&
                button.onClick.GetPersistentMethodName(index) == methodName)
                return;
        }

        if (methodName == nameof(PauseMenu.Reanudar))
            UnityEventTools.AddPersistentListener(button.onClick, ((PauseMenu)target).Reanudar);
        else if (methodName == nameof(PauseMenu.IrAlMenu))
            UnityEventTools.AddPersistentListener(button.onClick, ((PauseMenu)target).IrAlMenu);
        else if (methodName == nameof(MainMenu.Jugar))
            UnityEventTools.AddPersistentListener(button.onClick, ((MainMenu)target).Jugar);
        else if (methodName == nameof(MainMenu.Salir))
            UnityEventTools.AddPersistentListener(button.onClick, ((MainMenu)target).Salir);
    }

    static void SetReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        if (target == null)
            return;
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null)
            return;
        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static GameObject FindRootOrChild(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name)
                return root;
            Transform child = FindDeep(root.transform, name);
            if (child != null)
                return child.gameObject;
        }
        return null;
    }

    static Transform FindDeep(Transform root, string name)
    {
        if (root == null)
            return null;
        if (root.name == name)
            return root;
        foreach (Transform child in root)
        {
            Transform found = FindDeep(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}
