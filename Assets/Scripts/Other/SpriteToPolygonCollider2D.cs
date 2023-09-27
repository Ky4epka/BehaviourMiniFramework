using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Other
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class SpriteToPolygonCollider2D : MonoBehaviour
    {
        public Color Initial_MatchColor = Color.clear;
        public bool Initial_MatchByTransparent = true;

        protected Color imatchColor = Color.clear;
        protected bool imatchByTransparent = false;
        protected SpriteRenderer icachedRenderer = null;
        protected PolygonCollider2D icachedPolyCol = null;
        protected Sprite iLastSprite = null;

        protected const int CELL_SIBLINGS_COUNT = 8;
        protected static readonly Vector2Int[] CELL_SIBLINGS_OFFSETS = new Vector2Int[CELL_SIBLINGS_COUNT] {
                                                                                        new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1),
                                                                                        new Vector2Int(-1, 0),                         new Vector2Int(1, 0),
                                                                                        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
                                                                                        };

        protected LinkedListEx<PixelData> iPixelDataBuffer = new LinkedListEx<PixelData>();

        protected struct PixelData
        {
            public Color pixel;
            public Vector2Int position;

            public PixelData(Color _pixel, Vector2Int _position)
            {
                pixel = _pixel;
                position = _position;
            }

            public bool IsSibling(Vector2Int check_position, ref bool IsGreat)
            {
                Vector2Int delta = position - check_position;
                IsGreat = (delta.x > 0) && (delta.y > 0);

                return 
                       (Mathf.Abs(delta.x) <= 1) &&
                       (Mathf.Abs(delta.y) <= 1);
            }

            public bool IsSibling(PixelData check, ref bool IsGreat)
            {
                return IsSibling(check.position, ref IsGreat);
            }

            public Vector2 ToPolyPosition(int tex_width, int tex_height)
            {
                return new Vector2(position.x - tex_width / 2f, tex_height - position.y - tex_height / 2f);
            }

            public Vector2[] PixelToPolyLine(PixelData toData, int tex_width, int tex_height, float scale)
            {
                Vector2 pivot_pos = ToPolyPosition(tex_width, tex_height);
                Vector2 temp = pivot_pos;
                Vector2 dest_pos = toData.ToPolyPosition(tex_width, tex_height);
                pivot_pos = Vector2.Min(pivot_pos, dest_pos);
                pivot_pos.y -= scale;
                dest_pos = Vector2.Max(temp, dest_pos);
                dest_pos.y -= scale;
                Vector2 dir = (dest_pos - pivot_pos).normalized;

                if (MathKit.Vectors2DEquals(dir, Vector2.zero))
                    dir = Vector2.up;

                Vector2[] result = new Vector2[4];


                result[0].x = pivot_pos.x - scale * dir.y - scale * dir.x + scale;
                result[0].y = pivot_pos.y - scale * dir.x - scale * dir.y + scale;

                result[1].x = pivot_pos.x + scale * dir.y;
                result[1].y = pivot_pos.y + scale * dir.x;

                result[2].x = dest_pos.x + scale * dir.x + scale * dir.y;
                result[2].y = dest_pos.y + scale * dir.y + scale * dir.x;

                result[3].x = dest_pos.x + scale * dir.x;
                result[3].y = dest_pos.y + scale * dir.y;
                return result;
            }
        }

        protected Vector2[] ExtractAccessiblePath(LinkedListEx<PixelData> source, int tex_width, int tex_height)
        {
            if (source.Count == 0) return null;


            iPixelDataBuffer.Clear();
            iPixelDataBuffer.AddLast(source.First.Value);
            source.Remove(source.First);
            LinkedListNode<PixelData> buf_node = iPixelDataBuffer.First;

            while (buf_node != null)
            {
                LinkedListNode<PixelData> src_node = source.First;
                while (src_node != null)
                {
                    LinkedListNode<PixelData> next_node = src_node.Next;
                    bool IsGreat = false;
                    
                    if (src_node.Value.IsSibling(buf_node.Value, ref IsGreat))
                    {
                        LinkedListNode<PixelData> sort_node = iPixelDataBuffer.First;

                        while (sort_node != null)
                        {
                            LinkedListNode<PixelData> sort_next_node = sort_node.Next;
                            bool sort_isGreat = false;

                            if (sort_next_node == null)
                            {
                                if (MathKit.Vector2IntCompare(buf_node.Value.position, sort_node.Value.position) == -1)
                                    iPixelDataBuffer.AddBefore(sort_node, buf_node.Value);
                                else
                                    iPixelDataBuffer.AddAfter(sort_node, buf_node.Value);
                            }
                            else
                            {
                                if (sort_node.Value.IsSibling(buf_node.Value, ref sort_isGreat))
                                {
                                    if (sort_isGreat)
                                        iPixelDataBuffer.AddAfter(sort_node, buf_node.Value);
                                    else
                                        iPixelDataBuffer.AddBefore(sort_node, buf_node.Value);
                                }

                                break;
                            }

                            sort_node = sort_node.Next;
                        }

                        buf_node = iPixelDataBuffer.First;
                        source.Remove(src_node);
                    }

                    src_node = next_node;
                }

                buf_node = buf_node.Next;
            }

            Vector2[] result = new Vector2[iPixelDataBuffer.Count * 4];
            PixelData last = new PixelData(Color.clear, new Vector2Int(int.MaxValue, int.MinValue));
            int opt_count = 0;

            buf_node = iPixelDataBuffer.First;

            while (buf_node != null)
            {
                Debug.Log("Pixel: " + buf_node.Value.position);
                buf_node = buf_node.Next;
            }


            buf_node = iPixelDataBuffer.First;
            PixelData from = new PixelData(Color.clear, (buf_node != null) ? buf_node.Value.position : Vector2Int.zero);
            PixelData to = new PixelData(Color.clear, (buf_node != null) ? buf_node.Value.position : Vector2Int.zero);
            bool isStart = true;
            while (buf_node != null)
            {
                //Debug.Log("Buffer: " + buf_node.Value.position+" is parallel: "+ MathKit.Vectors2IntIsParallel(angle.position, buf_node.Value.position));
                bool is_parallel = MathKit.Vectors2IntIsParallel(last.position, buf_node.Value.position);

                if (!is_parallel || buf_node.Next == null)
                {
                    //Debug.Log("Is border " + last.position + ", " + buf_node.Value.position + ": " + MathKit.Vectors2IntIsParallel(last.position, buf_node.Value.position));
                    if (isStart)
                    {
                        from = buf_node.Value;
                        isStart = false;
                    }
                    else
                    {
                        to = last;
                        isStart = true;
                    }

                    if (buf_node.Next == null)
                    {
                        to = buf_node.Value;
                    }

                    if (isStart || buf_node.Next == null)
                    {
                        Debug.Log("Line: "+from.position + " - "+to.position);
                        Vector2[] vert_buf = from.PixelToPolyLine(to, tex_width, tex_height, 1f);

                        for (int i = 0; i < 4; i++)
                        {
                            result[opt_count++] = vert_buf[i];
                        }
                    }
                }

                last = buf_node.Value;
                buf_node = buf_node.Next;
            }

            Vector2[] optimized = new Vector2[opt_count];

            for (int i = 0; i < opt_count; i++)
            {
                optimized[i] = result[i];
            }

            return optimized;
        }

        protected LinkedListEx<PixelData> TexturePixelsByMatchColor(Texture2D texture, Rect matchRect, Color matchColor, bool matchByTransparent)
        {
            Color[] pixels = texture.GetPixels((int)matchRect.x, (int)matchRect.y, (int)matchRect.width, (int)matchRect.height);
            LinkedListEx<PixelData> result = new LinkedListEx<PixelData>();
            int tex_width = (int)matchRect.width;
            int tex_height = (int)matchRect.height;

            for (int i=0; i< pixels.Length; i++)
            {
                if ((matchByTransparent && (pixels[i].a != 0f)) || (pixels[i].Equals(matchColor)))
                    result.AddLast(new PixelData(pixels[i], new Vector2Int(i % tex_width, tex_height - 1 - i / tex_width)));
            }

            return result;
        }

        protected void GenPolygon()
        {
            LinkedListEx<PixelData> matches=TexturePixelsByMatchColor(icachedRenderer.sprite.texture, icachedRenderer.sprite.rect, imatchColor, imatchByTransparent);
            iPixelDataBuffer = new LinkedListEx<PixelData>();

            try
            {
                icachedPolyCol.pathCount = 0;
                Vector2[] path = null;
                do
                {
                    path = ExtractAccessiblePath(matches, (int)icachedRenderer.sprite.rect.width, (int)icachedRenderer.sprite.rect.height);

                    if (path == null || (path.Length == 0))
                        break;

                    icachedPolyCol.SetPath(icachedPolyCol.pathCount++, path);
                } while (true);
            }
            finally
            {
                iPixelDataBuffer = null;
            }
        }

        protected void Update()
        {
            if (iLastSprite != icachedRenderer.sprite)
            {
                iLastSprite = icachedRenderer.sprite;
                GenPolygon();
            }
        }

        protected void Awake()
        {
            icachedRenderer = GetComponent<SpriteRenderer>();
            icachedPolyCol = GetComponent<PolygonCollider2D>();
            imatchColor = Initial_MatchColor;
            imatchByTransparent = Initial_MatchByTransparent;
        }
    }
}
