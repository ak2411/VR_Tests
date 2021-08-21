using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class drawWhiteboard : MonoBehaviour
{
    [SerializeField] private GameObject pen;

    [SerializeField] private GameObject debug;

    private int textureHeight, textureWidth;

    private int pensize = 10;

    private Texture2D texture;
    private Renderer thisRenderer;
    private RaycastHit hit;
    private Color[] color;

    private int prevx, prevy;
    
    // Start is called before the first frame update
    void Start()
    {
        Texture2D originTexture = GetComponent<MeshRenderer>().materials[0].mainTexture as Texture2D;
        thisRenderer = GetComponent<Renderer>();
        SetColor();
        textureWidth = originTexture.width;
        textureHeight = originTexture.height;
        texture = (Texture2D) GameObject.Instantiate(originTexture);
        thisRenderer.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouching(pen.transform.position, pen.transform.forward, out hit))
        {
            // debug.transform.position = hit.point;
            var newHit = transform.InverseTransformPoint(hit.point);
            int x = (int) ((newHit.x) * textureWidth + textureWidth/2 - (pensize / 2));
            int y = (int) ((newHit.y) * textureHeight + textureHeight/2 - (pensize / 2));
            // string lerpPoints = "";
            if (prevx != 0 && prevy != 0)
            {
                float lerpCount = (1 / 0.05f);
                for (int i = 0; i <= lerpCount; i++)
                {
                    int lerpx = (int) Mathf.Lerp((float) prevx, (float) x, i/lerpCount);
                    int lerpy = (int) Mathf.Lerp((float) prevy, (float) y, i/lerpCount);
                    // lerpPoints += lerpx.ToString() + " " + lerpy.ToString() + "     ";
                    texture.SetPixels(lerpx, lerpy, pensize, pensize, color);
                }
            }
            // Debug.Log(lerpPoints);
            // texture.SetPixels(x, y, pensize, pensize, color);
            prevx = x;
            prevy = y;
            texture.Apply();
        }
        else
        {
            prevx = 0;
            prevy = 0;
        }
    }

    private bool isTouching(Vector3 penPos, Vector3 penDirection, out RaycastHit hit)
    {
        return Physics.Raycast(penPos, penDirection, out hit, 0.1f) && hit.collider.CompareTag("board");
    }

    private void SetColor()
    {
        color = Enumerable.Repeat<Color>(Color.blue, pensize * pensize).ToArray<Color>();
    }
}
