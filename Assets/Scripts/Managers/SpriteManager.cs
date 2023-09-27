using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using UnityEditor;

namespace Main.Managers
{
    [System.Serializable]
    public class SpriteList: ListEx<Sprite>
    {
        public Sprite[] Init_SpriteList = null;
        public List<string> Init_SpriteNames = null;

        protected void Init()
        {
            AddItemPredicate = Filter_AddItem;
        }

        public SpriteList() : base()
        {
            Init();
        }

        public SpriteList(IEnumerable<Sprite> collection) : base(collection)
        {
            Init();
        }

        public SpriteList(int capacity) : base(capacity)
        {
            Init();
        }

        public SpriteList(int capacity, bool auto_grow, int grow_quota) : base(capacity, auto_grow, grow_quota)
        {
            Init();
        }

        public Sprite ItemAt(string name)
        {
            int index = IndexByNameProp(name);

            if (index != Configuration.INVALID_INDEX)
                return ItemAt(index);

            return null;
        }

        public int IndexByNameProp(string name)
        {
            for (int i = 0; i < Count; i++)
                if (ItemAt(i).name.Equals(name))
                    return i;

            return Configuration.INVALID_INDEX;
        }

        protected bool Filter_AddItem(Sprite filtered)
        {
            return IndexByNameProp(filtered.name) == Configuration.INVALID_INDEX;
        }

        public void AddFromResources(IList<string> res_files, UnityEngine.Events.UnityAction<int, int> progress_callback = null /* 1p (int current) 2p (int total) */)
        {
            int counter = 0;
                        
            for (int i=0; i<res_files.Count; i++)
            {
                Sprite[] res = null;
                try
                {
                    res = Resources.LoadAll<Sprite>("Textures/"+res_files[i]);
                }
                catch (System.Exception e)
                {
                    GLog.LogException(e);
                }
                
                for (int j=0; j<res.Length; j++)
                {
                    Add(res[j]);
                }

                if (progress_callback != null)
                    progress_callback(counter, res_files.Count);
                                
                counter++;
            }
        }
    }

    public class GrouppedSprite
    {
        public string Group { get; protected set; }
        public string Id { get; protected set; }
        public Sprite Sprite { get; protected set; }

        public GrouppedSprite (string group, string id, Sprite sprite)
        {
            Group = group;
            Id = id;
            Sprite = sprite;
        }
    }

    public class SpriteManagerPopupWindowContent : PopupWindowContent
    {
        public SpriteManager SpriteManager { get; protected set; }
        public Vector2 SpritePreviewSize { get; set; }
        public Vector2 SpritePreviewSpacing { get; set; }
        public Vector2Int SpritesMatrixSizeInView { get; set; }

        protected Vector2 iScrollPosition = Vector2.zero;

        public SpriteManagerPopupWindowContent(SpriteManager manager)
        {
            SpriteManager = manager;
        }

        public Vector2 ContentSize
        {
            get
            {
                Vector2 spriteStep = SpritePreviewSize + SpritePreviewSpacing;

                return new Vector2(SpritesMatrixSizeInView.x * spriteStep.x,
                                   SpritesMatrixSizeInView.y * spriteStep.y);
            }
        }

        public override void OnGUI(Rect rect)
        {
            Vector2 curPos = rect.position + SpritePreviewSpacing;

            int counter = 0;
            Rect horzRect = rect;
            Vector2 spriteStep = SpritePreviewSize + SpritePreviewSpacing;

            iScrollPosition = EditorGUILayout.BeginScrollView(iScrollPosition);

            try
            {
                foreach (Sprite sprite in SpriteManager.Sprites)
                {
                    if (counter % SpritesMatrixSizeInView.x == 0)
                    {
                        if (counter > 0)
                        {
                            horzRect.x = 0;
                            horzRect.position += new Vector2(0, spriteStep.y);
                        }
                    }

                    horzRect.size = SpritePreviewSize;
                    //EditorGUI.DrawPreviewTexture(horzRect, sprite.texture);
                    //GUI.DrawTextureWithTexCoords(horzRect, sprite.texture, sprite.textureRect, true);
                    EditorGUILayout.ObjectField(sprite.name, sprite, typeof(Sprite), true, GUILayout.Width(200));
                    
                    horzRect.position += new Vector2(spriteStep.x, 0);
                    counter++;
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }
    }

    public class SpriteManager : BehaviourContainer
    {
        public const char PathDelimiter = '/';

        public SpriteList Sprites = new SpriteList();

        protected static SpriteManager iInstance = null;

        protected Dictionary<string, Sprite> iSpriteList = new Dictionary<string, Sprite>();

        public static SpriteManager Instance
        {
            get
            {
                if (iInstance == null)
                    iInstance = GameObject.FindObjectOfType<SpriteManager>();

                if (iInstance == null)
                    throw new System.NullReferenceException($"Could not find a {nameof(SpriteManager)}");

                DontDestroyOnLoad(iInstance);
                return iInstance;
            }
        }

        protected override void Awake()
        {
        }

        protected override void OnValidate()
        {
            foreach (Sprite sprite in Sprites.Init_SpriteList)
            {
                Sprites.Add(sprite);
            }

            Sprites.AddFromResources(Sprites.Init_SpriteNames);
        }

        [MenuItem("Test/Popup")]
        static void ShowPopup()
        {
            SpriteManagerPopupWindowContent popupContent = new SpriteManagerPopupWindowContent(SpriteManager.Instance);
            popupContent.SpritePreviewSize = new Vector2(32, 32);
            popupContent.SpritePreviewSpacing = new Vector2(5, 5);
            popupContent.SpritesMatrixSizeInView = new Vector2Int(3, 3);
            PopupWindow.Show(new Rect(Vector2.zero, popupContent.GetWindowSize()), popupContent);
        }
    }

}