using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colorPercent : MonoBehaviour
{
    //calculates the percentage of color shades in the image
    //important!!! it is necessary to change texture format to rgba 32 bit in inspector
    //(also FilterMode = Point and Texture Wrap Mode = Clamp, flag read/write enable = true)
    //only quad images

    public Text redText;
    public Text greenText;
    public Text blueText;
    public Text shadesGreyText;
    public Texture2D texture;
    public FilterMode filterMode = FilterMode.Point;
    public TextureWrapMode textureWrapMode = TextureWrapMode.Clamp;

    int maxPixel = 0;
    float r = 0;
    float g = 0;
    float b = 0;
    float shadesGrey = 0;
    Color color;

    private void Start()
    {
        if (texture != null)
        {
            texture.filterMode = filterMode;
            texture.wrapMode = textureWrapMode;
        }
        else
        {
            Debug.Log("Add Image");
        }
    }
    void Update()
    {
        if (texture != null)
        {
            if (redText != null && greenText != null && blueText != null)
            {
                if (maxPixel < texture.height * texture.width)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        for (int x = 0; x < texture.width; x++)
                        {
                            color = texture.GetPixel(x, y);
                            if (color.r > color.g && color.r > color.b)
                            {
                                r++;
                            }
                            if (color.g > color.r && color.g > color.b)
                            {
                                g++;
                            }
                            if (color.b > color.r && color.b > color.g)
                            {
                                b++;
                            }
                            if (color.b == color.r && color.b == color.g)
                            {
                                shadesGrey++;
                            }
                            float R = (r / (texture.height * texture.width)) * 100;
                            float G = (g / (texture.height * texture.width)) * 100;
                            float B = (b / (texture.height * texture.width)) * 100;
                            float ShadesGrey = (shadesGrey / (texture.height * texture.width)) * 100;
                            redText.text = "pixels of red hue count: " + R.ToString("0.0") + " percent";
                            greenText.text = "pixels of green hue count: " + G.ToString("0.0") + " percent";
                            blueText.text = "pixels of blue hue count: " + B.ToString("0.0") + " percent";
                            shadesGreyText.text = "grayscale pixels count: " + ShadesGrey.ToString("0.0") + " percent";
                            maxPixel++;
                        }
                    }
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    maxPixel = 0;
                }
            }
            else
            {
                Debug.Log("Add Texts");
            }
        }   
    }
}