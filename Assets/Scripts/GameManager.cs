using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int baseFollowers = 3;
    [SerializeField] private float followersScale = 0.8f;
    [SerializeField] private int baseMoney = 5;
    [SerializeField] private float moneyMultiplier = 3.5f;
    public PlayerStats player;
    public PlayerInventory playerInventory;
    public UIManager uiManager;
    [SerializeField] private marcoMovement playerMovement;
    [SerializeField] private SpawAudience spawAudience;
    [SerializeField] private TheoryValidator theoryValidator;

    private void Awake()
    {
        uiManager.BindPlayer(player);
    }

    public void giveMoney(int amount)
    {
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
        uiManager.SetNight();
        spawAudience.StartSpawningAudience();

    }

    public void EndSpeech()
    {
        playerMovement.SetTransitionState(false);
        GiveRewardsSpeech();
    }

    public void GiveRewardsSpeech()
    {
        int moneyReward = CalculateMoneyReward();
        //Debug.Log("Theory is correct: " + CheckIfTheoryIsCorrect(player.GetCurrentTheory()));
        int followersReward = CalculateFollowersReward(CheckIfTheoryIsCorrect(player.GetCurrentTheory()));
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
}