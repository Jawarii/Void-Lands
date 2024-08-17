using UnityEngine;
[System.Serializable]
public class ColorData
{
    public byte r, g, b, a;

    public ColorData(Color color)
    {
        r = (byte)(color.r * 255);
        g = (byte)(color.g * 255);
        b = (byte)(color.b * 255);
        a = (byte)(color.a * 255);
    }

    public Color ToColor()
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
