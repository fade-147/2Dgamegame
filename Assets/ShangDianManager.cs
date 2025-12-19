using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShangDianManager : MonoBehaviour
{
    public Button LeftButton;
    public Button RightButton;
    public Button JianButton;
    public GameObject NormalText;
    public GameObject OKText;
    public GameObject NoMoneyText;
    public GameObject DieText;
    private Character PlayerCharacter;
    public Button BackButton;
    public CharacterEvent CharacterEventSO;

    public GameObject LeftSrow;
    public GameObject RightSrow;
    public GameObject JianSrow;

    public GameObject FinishJian1;
    public GameObject FinishJian2;
    public GameObject FinishJian33;
    private void Awake()
    {
        LeftButton.onClick.AddListener(ToggleLeftButton);
        RightButton.onClick.AddListener(ToggleRightButton);
        JianButton.onClick.AddListener(ToggleJianButton);
        BackButton.onClick.AddListener(ToggleBackButton);
    }


    private void ToggleBackButton()
    {
        this.gameObject .SetActive(false);
    }
    private void ToggleJianButton()
    {
        int currentCoins = GameManager.Instance.CoinCount;
        if (currentCoins < 10)
        {
            StartCoroutine(ToggleLeftNoCoroutine());
            return;
        }
        GameManager.Instance.RemoveCoins(10);
        StartCoroutine(ToggleOKJianCoroutine());
    }

    private void ToggleRightButton()
    {
        PlayerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        if(PlayerCharacter != null)
        {
            if (PlayerCharacter.currentHealth / PlayerCharacter.maxHealth > 0.6)
            {
                PlayerCharacter.currentHealth -= (float)(PlayerCharacter.maxHealth* 0.6);
                CharacterEventSO.RaisedEvent(PlayerCharacter);
                StartCoroutine(ToggleOKRightCoroutine());
            }
            else
            {
                StartCoroutine(ToggleRightNoCoroutine());
            }
        }
    }

    private void ToggleLeftButton()
    {
        int currentCoins = GameManager.Instance.CoinCount;
        if (currentCoins < 5)
        {
            StartCoroutine(ToggleLeftNoCoroutine());
            return;
        }
        GameManager.Instance.RemoveCoins(5);
        StartCoroutine(ToggleOKLeftCoroutine());
    }

    private IEnumerator ToggleLeftNoCoroutine()
    {
        NormalText.SetActive (false);
        NoMoneyText.SetActive (true);
        yield return new WaitForSeconds(0.8f);
        NoMoneyText.SetActive(false);
        NormalText.SetActive(true);
    }
    private IEnumerator ToggleRightNoCoroutine()
    {
        NormalText.SetActive(false);
        DieText.SetActive (true);
        yield return new WaitForSeconds(0.8f);
        DieText.SetActive(false);
        NormalText.SetActive(true);
    }
    private IEnumerator ToggleOKLeftCoroutine()
    {
        NormalText.SetActive(false);
        OKText.SetActive(true);
        yield return new WaitForSeconds(1f);
        OKText.SetActive(false);
        NormalText.SetActive(true);
        LeftSrow.GetComponent<PickupSpawner>().DropItems();
        this.gameObject.SetActive(false);
    }
    private IEnumerator ToggleOKRightCoroutine()
    {
        NormalText.SetActive(false);
        OKText.SetActive(true);
        yield return new WaitForSeconds(1f);
        OKText.SetActive(false);
        NormalText.SetActive(true);
        RightSrow.GetComponent<PickupSpawner>().DropItems();
        this.gameObject.SetActive(false);
    }
    private IEnumerator ToggleOKJianCoroutine()
    {
        NormalText.SetActive(false);
        OKText.SetActive(true);
        yield return new WaitForSeconds(1f);
        JianButton.interactable = false;
        FinishJian1.SetActive(false);
        FinishJian2.SetActive(false);
        FinishJian33.SetActive(true);
        OKText.SetActive(false);
        NormalText.SetActive(true);
        JianSrow.GetComponent<PickupSpawner>().DropItems();
        this.gameObject.SetActive(false);
    }
}
