using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// Happy Glass 
/// Credit: Satyam Parkhi
/// Email: satyamparkhi@gmail.com
/// Facebook : https://www.facebook.com/satyamparkhi
/// Instagram : https://www.instagram.com/satyamparkhi/
/// Whatsapp : +91 7050225661
/// </summary>
[ExecuteInEditMode]
public class LockedLevels : MonoBehaviour {


    void Awake()
    {

        PlayerPrefs.SetInt("1", 1);
        UnLockLevels();
        
    }
    void Update()
    {
        transform.GetChild(0).gameObject.GetComponent<Text>().text = transform.name;
        transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = 75;
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void UnLockLevels()
    {
        if (PlayerPrefs.GetInt(gameObject.name) == 1)
        {
            transform.GetChild(0).gameObject.GetComponent<Text>().color = new Color(1,1,1,1) ;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
            transform.GetChild(0).gameObject.GetComponent<Text>().color = new Color(1, 1, 1, .5f);
        }
    }
    public void LevelMenu()
    {
        SceneManager.LoadScene(transform.name);
    }
}

