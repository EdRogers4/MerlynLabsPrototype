using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Sound")]
    public SoundLibrary scriptSoundLibrary;

    [Header("Cards")]
    [SerializeField] private CardPositioner scriptCardPositioner;
    [SerializeField] private CardSpawner scriptCardSpawner;
    [SerializeField] private CardsLibrary scriptCardsLibrary;
    public bool isPlantFeet;
    public bool isHiltPunch;
    public bool isKnightsResolve;
    public bool isFlurry;
    public bool isLightningArc;
    public int modifierPlantFeet;
    public int modifierHiltPunch;
    public int counterRoundhouse;

    [Header("Camera")]
    [SerializeField] private PerlinCameraShake scriptPerlinCameraShake;
    [SerializeField] private float cameraShakeTrauma;

    [Header("Game Status")]
    public bool isCardUnavailableToPlay;
    [SerializeField] private bool isEndTurnUnavailable;
    [SerializeField] private GameObject buttonEndTurn;
    [SerializeField] private GameObject deathScreen;

    [Header("Player")]
    public bool isPlayerDead;
    public int playerArmor;
    [SerializeField] private int playerVengeance;
    [SerializeField] private int playerHealth;
    [SerializeField] private int playerHealthMax;
    [SerializeField] private Image playerHealthFill;
    [SerializeField] private GameObject armorDisplay;
    [SerializeField] private GameObject vengeanceDisplay;
    [SerializeField] private TextMeshProUGUI textPlayerArmor;
    [SerializeField] private TextMeshProUGUI textPlayerVengeance;
    [SerializeField] private TextMeshProUGUI textPlayerHealth;
    [SerializeField] private Animator animatorPlayer;
    [SerializeField] private ParticleSystem[] particlePlayerDamage;
    [SerializeField] private ParticleSystem[] particlePlayerHeal;
    [SerializeField] private ParticleSystem particlePlayerVengeance;
    [SerializeField] private ParticleSystem particlePlayerArmor;
    public int playerEnergy;
    private float playerFillDecrease;

    [Header("Enemy")]
    public bool[] isEnemyDead;
    [SerializeField] private int[] enemyDamage;
    [SerializeField] private int[] enemyHealth;
    [SerializeField] private int[] enemyHealthMax;
    [SerializeField] private int[] countTurnsLeftToSleep;
    [SerializeField] private GameObject[] sleepDisplay;
    [SerializeField] private TextMeshProUGUI[] textSleepCounter;
    [SerializeField] private Image[] enemyHealthFill;
    [SerializeField] private TextMeshProUGUI[] textEnemyHealth;
    [SerializeField] private TextMeshProUGUI[] textEnemyZZZ;
    [SerializeField] private Animator[] animatorEnemy;
    [SerializeField] private bool[] isEnemySleep;
    [SerializeField] private ParticleSystem[] particleSlashEnemy0;
    [SerializeField] private ParticleSystem[] particleSlashEnemy1;
    [SerializeField] private ParticleSystem[] particleHitEnemy0;
    [SerializeField] private ParticleSystem[] particleHitEnemy1;
    [SerializeField] private ParticleSystem[] particleBlueEnemy0;
    [SerializeField] private ParticleSystem[] particleBlueEnemy1;
    [SerializeField] private ParticleSystem[] particleEnergyEnemy0;
    [SerializeField] private ParticleSystem[] particleEnergyEnemy1;
    [SerializeField] private ParticleSystem[] particleEnemySleep;
    [SerializeField] private ParticleSystem[] particleEnemyLightning0;
    [SerializeField] private ParticleSystem[] particleEnemyLightning1;
    private float enemyFillDecrease;
    private bool isShowEnemyZZZ;
    private int countEnemyZZZ;

    [Header("Energy")]
    [SerializeField] private TextMeshProUGUI textPlayerEnergy;

    [Header("Potions")]
    public bool isUsePotion;
    [SerializeField] private GameObject[] potions;
    [SerializeField] private GameObject potionMenu;
    [SerializeField] private Animator animatorSelectEnemy;
    private int selectedPotion;

    [Header("Popups")]
    [SerializeField] private GameObject tooltipAbility;
    [SerializeField] private GameObject tooltipSleepPotion;
    [SerializeField] private GameObject[] tooltipSleep;
    [SerializeField] private TextMeshProUGUI textTooltipDescription;
    [SerializeField] private Animator[] animatorPopupTextEnemy;
    [SerializeField] private Animator animatorPopupTextPlayerDamage;
    [SerializeField] private Animator animatorPopupTextPlayerHealth;
    [SerializeField] private TextMeshProUGUI[] textPopupDamageEnemy;
    [SerializeField] private TextMeshProUGUI textPopupHealthPlayer;

    [Header("Test")]
    [SerializeField] private bool testDamageEnemy;

    private void Start()
    {
        playerFillDecrease = 1f / playerHealthMax * playerHealthFill.rectTransform.sizeDelta.x;
        enemyFillDecrease = 1f / enemyHealthMax[0] * enemyHealthFill[0].rectTransform.sizeDelta.x;
        FillEnergy();
    }

    private void FillEnergy()
    {
        playerEnergy = 3;
        textPlayerEnergy.text = playerEnergy + "";
    }

    public void UseEnergy(int cost)
    {
        playerEnergy = playerEnergy - cost;
        textPlayerEnergy.text = playerEnergy + "";
    }

    public void EndTurn()
    {
        if (!isEndTurnUnavailable)
        {
            isEndTurnUnavailable = true;
            scriptSoundLibrary.SoundEndTurn();

            for (int i = scriptCardPositioner.listCardTransform.Count - 1; i >= 0; i--)
            {
                var thisCard = scriptCardPositioner.listCardTransform[i].gameObject;
                scriptCardPositioner.listCardTransform.Remove(scriptCardPositioner.listCardTransform[i]);
                scriptCardPositioner.AssignCurrentCardPositions(scriptCardPositioner.listCardTransform.Count);
                Destroy(thisCard);
            }

            StartCoroutine(EnemyAttack(enemyDamage[0]));
        }
    }

    private void CheckCardToPlayStatus()
    {
        if (isCardUnavailableToPlay)
        {
            isCardUnavailableToPlay = false;
        }
    }

    private IEnumerator EnemyAttack(int damage)
    {
        for (int h = 0; h < 2; h++)
        {
            if (!isEnemyDead[h] && !isEnemySleep[h] && !isPlayerDead)
            {
                yield return new WaitForSeconds(1.0f);
                animatorEnemy[h].SetBool("isAttack", true);
                scriptSoundLibrary.SoundEnemyAttack();
                scriptSoundLibrary.SoundEnemySlash();
                particlePlayerDamage[Random.Range(0, particlePlayerDamage.Length)].Play();
                yield return new WaitForSeconds(0.75f);
                animatorPlayer.SetBool("isTakeDamage", true);
                animatorPopupTextPlayerDamage.SetBool("isShow", true);

                if (playerArmor <= 0)
                {
                    for (int i = 0; i < damage; i++)
                    {
                        playerHealth -= 1;
                        var newWidth = playerHealthFill.rectTransform.sizeDelta.x - playerFillDecrease;
                        playerHealthFill.rectTransform.sizeDelta = new Vector2(newWidth, playerHealthFill.rectTransform.sizeDelta.y);
                        yield return new WaitForSeconds(0.03f);
                        textPlayerHealth.text = playerHealth + "/" + playerHealthMax;

                        if (playerHealth <= 0)
                        {
                            StartCoroutine(PlayerDeath());
                            break;
                        }
                    }
                }
                else
                {
                    scriptSoundLibrary.SoundPlayerBlock();

                    for (int i = 0; i < damage; i++)
                    {
                        if (playerArmor <= 0)
                        {
                            var remainingDamage = enemyDamage[h] - (i + 1);

                            for (int j = 0; j < remainingDamage; j++)
                            {
                                playerHealth -= 1;
                                var newWidth = playerHealthFill.rectTransform.sizeDelta.x - playerFillDecrease;
                                playerHealthFill.rectTransform.sizeDelta = new Vector2(newWidth, playerHealthFill.rectTransform.sizeDelta.y);
                                yield return new WaitForSeconds(0.03f);
                                textPlayerHealth.text = playerHealth + "/" + playerHealthMax;

                                if (playerHealth <= 0)
                                {
                                    StartCoroutine(PlayerDeath());
                                    break;
                                }
                            }
                            break;
                        }

                        playerArmor -= 1;
                        textPlayerArmor.text = playerArmor + "";

                        if (playerArmor <= 0)
                        {
                            armorDisplay.SetActive(false);
                            scriptSoundLibrary.SoundPlayerArmorBreak();
                        }

                        yield return new WaitForSeconds(0.1f);
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (isEnemySleep[h])
            {
                countTurnsLeftToSleep[h] -= 1;
                textSleepCounter[h].text = "" + countTurnsLeftToSleep[h];

                if (countTurnsLeftToSleep[h] <= 0)
                {
                    isEnemySleep[h] = false;
                    textEnemyZZZ[h].text = "";
                    animatorEnemy[h].SetBool("isSleep", false);
                    sleepDisplay[h].SetActive(false);
                }
            }
        }

        scriptCardSpawner.SpawnCards();
        FillEnergy();
        isEndTurnUnavailable = false;
    }

    private IEnumerator PlayerDeath()
    {
        scriptSoundLibrary.SoundPlayerDeath();
        animatorPlayer.SetBool("isDeath", true);
        isPlayerDead = true;
        buttonEndTurn.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        deathScreen.SetActive(true);
    }

    public void PlayerAttack(int damage, int enemyToAttack)
    {
        animatorPlayer.SetBool("isAttack", true);
        StartCoroutine(DamageEnemy(damage, enemyToAttack));
    }

    public IEnumerator DamageEnemy(int damage, int enemyToAttack)
    {
        if (isFlurry)
        {
            UseEnergy(1);
        }

        if (counterRoundhouse > 0)
        {
            counterRoundhouse -= 1;
        }

        if (!isLightningArc)
        {
            scriptSoundLibrary.SoundPlayerAttack();

            if (enemyToAttack == 0)
            {
                particleSlashEnemy0[Random.Range(0, particleSlashEnemy0.Length)].Play();
                //particleHitEnemy0[Random.Range(0, particleHitEnemy0.Length)].Play();
                particleBlueEnemy0[Random.Range(0, particleBlueEnemy0.Length)].Play();
                particleEnergyEnemy0[Random.Range(0, particleEnergyEnemy0.Length)].Play();
            }
            else
            {
                particleSlashEnemy1[Random.Range(0, particleSlashEnemy1.Length)].Play();
                //particleHitEnemy1[Random.Range(0, particleHitEnemy1.Length)].Play();
                particleBlueEnemy1[Random.Range(0, particleBlueEnemy1.Length)].Play();
                particleEnergyEnemy1[Random.Range(0, particleEnergyEnemy1.Length)].Play();
            }
        }

        yield return new WaitForSeconds(0.75f);

        if (isEnemySleep[enemyToAttack])
        {
            isEnemySleep[enemyToAttack] = false;
            countTurnsLeftToSleep[enemyToAttack] = 0;
            sleepDisplay[enemyToAttack].SetActive(false);
            textEnemyZZZ[enemyToAttack].text = "";
            animatorEnemy[enemyToAttack].SetBool("isSleep", false);
            scriptSoundLibrary.SoundEnemyWakeUp();
        }

        scriptSoundLibrary.SoundEnemyTakeDamage();
        animatorEnemy[enemyToAttack].SetBool("isTakeDamage", true);
        animatorPopupTextEnemy[enemyToAttack].SetBool("isShow", true);
        textPopupDamageEnemy[enemyToAttack].text = "+" + damage + " Damage!";

        for (int i = 0; i < damage; i++)
        {
            scriptPerlinCameraShake.CameraShake(cameraShakeTrauma);
            enemyHealth[enemyToAttack] -= 1;
            var newWidth = enemyHealthFill[enemyToAttack].rectTransform.sizeDelta.x - enemyFillDecrease;
            enemyHealthFill[enemyToAttack].rectTransform.sizeDelta = new Vector2(newWidth, enemyHealthFill[enemyToAttack].rectTransform.sizeDelta.y);
            textEnemyHealth[enemyToAttack].text = enemyHealth[enemyToAttack] + "/" + enemyHealthMax[enemyToAttack];
            yield return new WaitForSeconds(0.1f);

            if (enemyHealth[enemyToAttack] <= 0)
            {
                animatorEnemy[enemyToAttack].SetBool("isDeath", true);
                isEnemyDead[enemyToAttack] = true;
                scriptSoundLibrary.SoundEnemyDeath();

                if (enemyToAttack == 0)
                {
                    scriptCardPositioner.enemyTargeted = 2;
                }
                else
                {
                    scriptCardPositioner.enemyTargeted = 1;
                }

                if (playerVengeance > 0)
                {
                    yield return new WaitForSeconds(2.0f);
                    StartCoroutine(GainHealth());
                }

                if (isEnemyDead[0] && isEnemyDead[1])
                {
                    StartCoroutine(RespawnEnemies());
                }

                break;
            }
        }

        if (isHiltPunch)
        {
            isHiltPunch = false;

            if (modifierHiltPunch > 1)
            {
                modifierHiltPunch -= 1;
                scriptCardsLibrary.cardDescription[5] = "Deal " + modifierHiltPunch + " damage.  Lower this cards damage by 1 each use during this combat.";
            }
        }

        if (isFlurry && !isEnemyDead[enemyToAttack] && playerEnergy > 0)
        {
            PlayerAttack(6, enemyToAttack);
        }
        else if (counterRoundhouse > 0 && !isEnemyDead[enemyToAttack])
        {
            PlayerAttack(7, enemyToAttack);
        }
        else
        {
            if (isFlurry)
            {
                isFlurry = false;
            }

            CheckCardToPlayStatus();
        }
    }

    public IEnumerator LightningArc(int enemyToAttack)
    {
        StartCoroutine(DamageEnemy(10, enemyToAttack));
        StartCoroutine(PlayLightningEffects(enemyToAttack));
        yield return new WaitForSeconds(0.5f);

        if (enemyToAttack == 0)
        {
            enemyToAttack = 1;
        }
        else
        {
            enemyToAttack = 0;
        }

        if (!isEnemyDead[enemyToAttack])
        {
            StartCoroutine(DamageEnemy(5, enemyToAttack));
        }

        yield return new WaitForSeconds(2.5f);
        isLightningArc = false;
    }

    private IEnumerator PlayLightningEffects(int enemyToAttack)
    {
        scriptSoundLibrary.SoundLightning();

        for (int i = 0; i < particleEnemyLightning0.Length; i++)
        {
            if (enemyToAttack == 0)
            {
                particleEnemyLightning0[i].Play();
            }
            else
            {
                particleEnemyLightning1[i].Play();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator GainHealth()
    {
        animatorPopupTextPlayerHealth.SetBool("isShow", true);
        textPopupHealthPlayer.text = "+" + playerVengeance + "HP!";
        scriptSoundLibrary.SoundHeal();

        for (int i = 0; i < particlePlayerHeal.Length; i++)
        {
            particlePlayerHeal[i].Play();
        }

        for (int i = 0; i < playerVengeance; i++)
        {
            if (playerHealth >= playerHealthMax)
            {
                break;
            }

            playerHealth += 1;
            var newWidth = playerHealthFill.rectTransform.sizeDelta.x + playerFillDecrease;
            playerHealthFill.rectTransform.sizeDelta = new Vector2(newWidth, playerHealthFill.rectTransform.sizeDelta.y);
            textPlayerHealth.text = playerHealth + "/" + playerHealthMax;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public IEnumerator GainArmor(int defense)
    {
        if (isKnightsResolve)
        {
            UseEnergy(1);
        }

        particlePlayerArmor.Play();
        scriptSoundLibrary.SoundGainArmor();

        for (int i = 0; i < defense; i++)
        {
            playerArmor += 1;

            if (playerArmor > 0)
            {
                armorDisplay.SetActive(true);
            }

            textPlayerArmor.text = "" + playerArmor;
            yield return new WaitForSeconds(0.1f);
        }

        if (isPlantFeet)
        {
            isPlantFeet = false;
            modifierPlantFeet = modifierPlantFeet * 2;
            scriptCardsLibrary.cardDescription[3] = "Gain " + modifierPlantFeet + " defense.  Doubled each use for the remainder of combat.";
        }

        if (isKnightsResolve && playerEnergy > 0)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GainArmor(6));
        }
        else
        {
            if (isKnightsResolve)
            {
                isKnightsResolve = false;
            }

            CheckCardToPlayStatus();
        }
    }

    public IEnumerator GainVengeance(int vengeance, int enemyTargeted)
    {
        particlePlayerVengeance.Play();
        scriptSoundLibrary.SoundGainVengeance();

        for (int i = 0; i < vengeance; i++)
        {
            playerVengeance += 1;

            if (!vengeanceDisplay.activeSelf)
            {
                vengeanceDisplay.SetActive(true);
            }

            textPlayerVengeance.text = "" + playerVengeance;
            yield return new WaitForSeconds(0.1f);
        }

        PlayerAttack(6, enemyTargeted);
    }

    private IEnumerator RespawnEnemies()
    {
        yield return new WaitForSeconds(0.5f);
        scriptSoundLibrary.SoundEnemySpawn();

        for (int i = 0; i < 2; i++)
        {
            animatorEnemy[i].SetBool("isDeath", false);
            enemyHealth[i] = enemyHealthMax[i];
            enemyHealthFill[i].rectTransform.sizeDelta = new Vector2(250, enemyHealthFill[i].rectTransform.sizeDelta.y);
            textEnemyHealth[i].text = enemyHealth[i] + "/" + enemyHealthMax[i];
            isEnemyDead[i] = false;
        }
    }

    public void TogglePotionMenu(int currentPotion)
    {
        selectedPotion = currentPotion;

        if (potionMenu.activeSelf)
        {
            potionMenu.SetActive(false);
            scriptSoundLibrary.SoundPotionClose();
        }
        else
        {
            potionMenu.SetActive(true);
            scriptSoundLibrary.SoundPotionSelect();
        }
    }

    public void UsePotion()
    {
        isUsePotion = true;
        TogglePotionMenu(selectedPotion);
        animatorSelectEnemy.SetBool("isShow", true);
        scriptSoundLibrary.SoundPotionUse();
    }

    public void DiscardPotion()
    {
        Destroy(potions[selectedPotion]);
        TogglePotionMenu(selectedPotion);
        scriptSoundLibrary.SoundPotionDiscard();
    }

    public void PutEnemyToSleep(int index)
    {
        if (isUsePotion)
        {
            isUsePotion = false;
            isEnemySleep[index] = true;
            animatorSelectEnemy.SetBool("isShow", false);
            animatorEnemy[index].SetBool("isSleep", true);
            Destroy(potions[selectedPotion]);
            particleEnemySleep[index].Play();
            scriptSoundLibrary.SoundSleep();
            countTurnsLeftToSleep[index] = 3;
            sleepDisplay[index].SetActive(true);
            textSleepCounter[index].text = "" + countTurnsLeftToSleep[index];

            if (!isShowEnemyZZZ)
            {
                isShowEnemyZZZ = true;
                StartCoroutine(EnemyShowZZZ());
            }
        }
    }

    private IEnumerator EnemyShowZZZ()
    {
        for (int i = 0; i < isEnemySleep.Length; i++)
        {
            if (isEnemySleep[i])
            {
                switch (countEnemyZZZ)
                {
                    case 0:
                        textEnemyZZZ[i].text = "z";
                        break;
                    case 1:
                        textEnemyZZZ[i].text = "zZ";
                        break;
                    case 2:
                        textEnemyZZZ[i].text = "zZz";
                        break;
                    case 3:
                        textEnemyZZZ[i].text = "";
                        break;
                }
            }
        }

        if (countEnemyZZZ >= 3)
        {
            countEnemyZZZ = 0;
        }
        else
        {
            countEnemyZZZ += 1;
        }

        yield return new WaitForSeconds(0.5f);

        if (isEnemySleep[0] || isEnemySleep[1])
        {
            StartCoroutine(EnemyShowZZZ());
        }
        else
        {
            isShowEnemyZZZ = false;
        }
    }

    public void ShowTooltip()
    {
        tooltipAbility.SetActive(true);
        textTooltipDescription.text = "Each time you kill an enemy, gain " + playerVengeance + " HP";
        scriptSoundLibrary.SoundTooltipShow();
    }

    public void HideTooltip()
    {
        tooltipAbility.SetActive(false);
        scriptSoundLibrary.SoundTooltipHide();
    }

    public void ShowTooltipPotion()
    {
        tooltipSleepPotion.SetActive(true);
        scriptSoundLibrary.SoundTooltipShow();
    }

    public void HideTooltipPotion()
    {
        tooltipSleepPotion.SetActive(false);
        scriptSoundLibrary.SoundTooltipHide();
    }

    public void ShowTooltipSleep(int index)
    {
        tooltipSleep[index].SetActive(true);
        scriptSoundLibrary.SoundTooltipShow();
    }

    public void HideTooltipSleep(int index)
    {
        tooltipSleep[index].SetActive(false);
        scriptSoundLibrary.SoundTooltipHide();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }    

    private void Update()
    {
        
    }
}
