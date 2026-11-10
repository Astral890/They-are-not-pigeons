using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int baseFollowers = 1000;
    [SerializeField] private float followersMultiplier = 3.5f;
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
        Debug.Log("Money Reward: " + moneyReward);
        giveMoney(moneyReward);
        Debug.Log("Theory is correct: " + CheckIfTheoryIsCorrect(player.GetCurrentTheory()));
    }

    private int CalculateFollowersReward()
    {
        
        int totalFollowers = baseFollowers + player.GetFollowers();
        return totalFollowers;
    }

    private int CalculateMoneyReward()
    {
        float reward = baseMoney + (moneyMultiplier * Mathf.Sqrt(Mathf.Max(0, player.GetFollowers())));
        return Mathf.FloorToInt(reward);
    }

    private bool CheckIfTheoryIsCorrect(string theory)
    {
        return theoryValidator.IsTheoryValid(theory);
    }
}