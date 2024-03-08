
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

[RequireComponent(typeof(Collider2D))]
public class CSButtonScript : MonoBehaviour
{
    public int buttonID;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cursor"))
        {
            print("collided");
            collision.GetComponent<CursorScript>().activeButton = buttonID;
            panel.GetComponent<Image>().color = new Color(.4f, .4f, .4f, .4f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Cursor"))
        {
            collision.GetComponent<CursorScript>().activeButton = -1;
            panel.GetComponent<Image>().color = new Color(1f, 1f, 1f, .4f);
        }
    }
}
