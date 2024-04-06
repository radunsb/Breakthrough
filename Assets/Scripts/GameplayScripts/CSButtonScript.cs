//Script for the panel buttons on the character select screen

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(Collider2D))]
public class CSButtonScript : MonoBehaviour
{
    public int buttonID;
    public GameObject panel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cursor"))
        {
            //Tell the cursor which button it is over
            collision.GetComponent<CursorScript>().activeButton = buttonID;
            //Darken the panel we're hovering over
            panel.GetComponent<Image>().color = new Color(.4f, .4f, .4f, .4f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cursor"))
        {
            //Tell the cursor it's no longer hovering over a button
            collision.GetComponent<CursorScript>().activeButton = -1;
            //Reset the color of the panel
            panel.GetComponent<Image>().color = new Color(1f, 1f, 1f, .4f);
        }
    }
}
