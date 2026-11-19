using System;
using UnityEngine;

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
    public int numDays = 1;
    public int followersReward = 0;
    [SerializeField] private HTTPHandler httpHandler;

    private void Awake()
    {
        uiManager.BindPlayer(player);
    }

    public void giveMoney(int amount)
    {
        Debug.Log("idHttp: " + httpHandler.datosUsuario.idUsuario.ToString());
        player.AddMoney(amount);
        uiManager.updateMoney(player.GetMoney());
    }

    public void giveFollowers(int amount)
    {
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
        isNight = false;
        slot1.ClearSlot();
        slot2.ClearSlot();
        slot3.ClearSlot();
        theoryManager.ResetTheory();
        player.SetCurrentTheory("None");
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