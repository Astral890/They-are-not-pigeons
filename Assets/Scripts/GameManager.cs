using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int baseFollowers = 3;
    [SerializeField] public float followersScale = 0.8f;
    [SerializeField] private int baseMoney = 5;
    [SerializeField] public float moneyMultiplier = 3.5f;
    [SerializeField] private TheorySlot slot1;
    [SerializeField] private TheorySlot slot2;
    [SerializeField] private TheorySlot slot3;
    public PlayerStats player;
    public PlayerInventory playerInventory;
    public UIManager uiManager;
    public bool isNight = false;
    [SerializeField] private marcoMovement playerMovement;
    [SerializeField] private SpawAudience spawAudience;
    [SerializeField] private TheoryValidator theoryValidator;
    [SerializeField] private TheoryManager theoryManager;
    [SerializeField] private SpeechManager speechManager;
    [SerializeField] private StoreTheories storeTheories;
    [SerializeField] private ResultPanelUI resultPanel;
    public int numDays = 1;
    public int followersReward = 0;
    [SerializeField] private HTTPHandler httpHandler;
    [SerializeField] private SwapTrees treeSwap;
    [SerializeField, Min(2)] private int daySwitch = 3;


    private void Awake()
    {
        uiManager.BindPlayer(player);
    }

    private void Start()
    {
        if (SaveSystem.ExistsSave())
        {
            Cargar();
        }
    }

    private void Guardar()
    {
        DatosGuardado datos = new DatosGuardado
        {
            currentMoney = player.GetMoney(),
            hunger = player.GetHunger(),
            followers = player.GetFollowers(),
            cohesion = player.cohesion,
            username = player.username,
            conceptInfos = new List<ConceptInfo>(player.conceptInfos),
            shopConcepts = new List<ConceptInfo>(storeTheories.shopConcepts),
            datosUsuario = JsonUtility.FromJson<HTTPHandler.DatosUsuario>(JsonUtility.ToJson(httpHandler.datosUsuario))
        };

        SaveSystem.SaveGame(datos);
    }

    private void Cargar()
    {
        DatosGuardado datos = SaveSystem.LoadGame();
        player.SetMoney(datos.currentMoney);
        player.SetHunger(datos.hunger);
        player.SetFollowers(datos.followers);
        player.cohesion = datos.cohesion;
        player.username = datos.username;
        player.conceptInfos = new List<ConceptInfo>(datos.conceptInfos);
        storeTheories.shopConcepts = new List<ConceptInfo>(datos.shopConcepts);
        httpHandler.datosUsuario = JsonUtility.FromJson<HTTPHandler.DatosUsuario>(JsonUtility.ToJson(datos.datosUsuario));
    }

    public void giveMoney(int amount)
    {
        Debug.Log("idHttp: " + httpHandler.datosUsuario.idUsuario.ToString());
        player.AddMoney(amount);
        uiManager.updateMoney(player.GetMoney());
    }

    public void giveFollowers(int amount)
    {
        if(amount == 0) player.UpdateCohesion(25);
        player.AddFollowers(amount);
        uiManager.updateFollowers(player.GetFollowers());
    }

    public void takeMoney(int amount)
    {
        player.RemoveMoney(amount);
        uiManager.updateMoney(player.GetMoney());
    }

    public void StartSpeech()
    {
        playerMovement.SetCamaraOnAudience();
        playerMovement.SetTransitionState(true);
        isNight = true;
        uiManager.SetNight();
        spawAudience.StartSpawningAudience();

    }

    public void EndSpeech()
    {
        playerMovement.SetTransitionState(false);
        speechManager.ResetStatus();
        GiveRewardsSpeech();
        UploadUserInformation();
    }

    public void GiveRewardsSpeech()
    {
        int moneyReward = CalculateMoneyReward();
        //Debug.Log("Theory is correct: " + CheckIfTheoryIsCorrect(player.GetCurrentTheory()));
        followersReward = CalculateFollowersReward(CheckIfTheoryIsCorrect(player.GetCurrentTheory()));
        Debug.Log("Money Reward: " + moneyReward);
        Debug.Log("Followers Reward: " + followersReward);
        giveMoney(moneyReward);
        giveFollowers(followersReward);
    }

    private int CalculateMoneyReward()
    {
        float reward = baseMoney + (moneyMultiplier * Mathf.Sqrt(Mathf.Max(0, player.GetFollowers())));
        return Mathf.FloorToInt(reward);
    }

    private int CalculateFollowersReward(bool isTheoryCorrect)
    {
        int amount = 0;
        if (isTheoryCorrect)
        {
            float followersCalc = baseFollowers + (followersScale * Mathf.Sqrt(Mathf.Max(0,player.GetFollowers())));
            amount = Mathf.RoundToInt(followersCalc);
        }
        return amount;
    }

    private bool CheckIfTheoryIsCorrect(string theory)
    {
        return theoryValidator.IsTheoryValid(theory);
    }

    public void NextDay()
    {
        numDays++;

        if (numDays == daySwitch && treeSwap != null)
        {
            treeSwap.SwapPlaces();
        }

        isNight = false;
        slot1.ClearSlot();
        slot2.ClearSlot();
        slot3.ClearSlot();
        theoryManager.ResetTheory();
        player.SetCurrentTheory("None");
        spawAudience.ClearAudiendie();
        if(numDays >= 10) resultPanel.Show(MatchOutcome.Victory, player.GetMoney(), player.GetFollowers(), numDays);
    }

    private void UploadUserInformation()
    {
        httpHandler.UpdateUserData(player.username, player.GetFollowers(), player.cohesion, player.GetMoney());
        Action alTerminarStats = () =>
        {
            if (player.GetCurrentTheory() != "Weak theory")
            {
                Debug.Log("Entro en asignar video");
                httpHandler.AsingVideoToUser(player.GetCurrentTheory());
            }
        };

        if (httpHandler.datosUsuario.idUsuario == 0)
        {
            httpHandler.UploadUser(() => httpHandler.UploadStats(alTerminarStats));
        }
        else
        {
            httpHandler.UploadStats(alTerminarStats);
        }
    }

    public void RemoveConceptFromUser(string descriptionText)
    {
        ConceptInfo conceptDelete = player.conceptInfos.Find(c => c.description == descriptionText);
        Debug.Log("Concepto a borrar: " + conceptDelete.description);
        player.conceptInfos.Remove(conceptDelete);
    }
}