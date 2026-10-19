using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.XR;
using TMPro;
using System;

public class AdsManager : MonoBehaviour
{
    public TextMeshProUGUI xpText;
    private bool canShowAd = true;

    public Profile profile;

    public static bool wasInMenu = false;

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

    void Start()
    {
        if (wasInMenu)
        {
            ShowInterstitialAd();
        }

        if (!wasInMenu)
        {
            wasInMenu = true;
        }

        string lastAdDate = PlayerPrefs.GetString("LastAdDate", "");
        DateTime currentDate = DateTime.Now;

        if (lastAdDate == "")
        {
            PlayerPrefs.SetString("LastAdDate", currentDate.ToString());
        }
        else
        {
            DateTime lastDate = DateTime.Parse(lastAdDate);

            if (currentDate.DayOfYear != lastDate.DayOfYear)
            {
                PlayerPrefs.SetString("LastAdDate", currentDate.ToString());
                PlayerPrefs.SetInt("WatchedAds", 0);
            }
        }

        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        int xp = GetXp();
        if (xp == 0)
        {
            xpText.text = "More ads tomorrow";
            canShowAd = false;
        }
        else
        {
            xpText.text = "Get " + xp + " XP";
        }
    }

    // === INTERSTITIAL AD (Tranzitivna reklama) ===
    public void LoadInterstitialAd()
    {
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial ad loaded.");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed.");
                LoadInterstitialAd();
            };
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet.");
        }
    }

    // === REWARDED AD (Nagrađujuća reklama) ===
    public void LoadRewardedAd()
    {
        AdRequest adRequest = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded ad loaded.");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed.");

                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardedAd(int xp)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                PlayerPrefs.SetInt("WatchedAds", PlayerPrefs.GetInt("WatchedAds", 0) + 1);

                PlayerPrefs.SetInt("Exp", PlayerPrefs.GetInt("Exp", 0) + xp);

                CheckNewLevel();
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    public int GetXp()
    {
        int watchedAdsToday = PlayerPrefs.GetInt("WatchedAds", 0);

        if (watchedAdsToday == 10)
        {
            return 0;
        }

        int playerLevel = PlayerPrefs.GetInt("Level", 0);
        int xp = 0;

        if (watchedAdsToday == 0)
        {
            xp = 5;
        }
        else if (watchedAdsToday == 1)
        {
            xp = 10;
        }
        else if (watchedAdsToday == 2)
        {
            xp = 15;
        }
        else if (watchedAdsToday == 3)
        {
            xp = 20;
        }
        else if (watchedAdsToday == 4)
        {
            xp = 30;
        }
        else if (watchedAdsToday == 5)
        {
            xp = 40;
        }
        else if (watchedAdsToday == 6)
        {
            xp = 50;
        }
        else if (watchedAdsToday == 7)
        {
            xp = 60;
        }
        else if (watchedAdsToday == 8)
        {
            xp = 80;
        }
        else if (watchedAdsToday == 9)
        {
            xp = 100;
        }

        if (playerLevel < 51)
        {
            xp *= 1;
        }
        else if (playerLevel < 102)
        {
            xp *= 2;
        }
        else if (playerLevel < 153)
        {
            xp *= 3;
        }
        else if (playerLevel < 204)
        {
            xp *= 4;
        }
        else if (playerLevel < 255)
        {
            xp *= 5;
        }
        else
        {
            xp *= 6;
        }

        return xp;
    }

    public void CheckNewLevel()
    {
        int xp = GetXp();
        if (xp == 0)
        {
            xpText.text = "More ads tomorrow";
            canShowAd = false;
        }
        else
        {
            xpText.text = "Get " + xp + " XP";
        }

        int neededExp = 0;
        int playerLevel = PlayerPrefs.GetInt("Level", 0);
        if (playerLevel < 51)
        {
            neededExp = 100;
        }
        else if (playerLevel < 102)
        {
            neededExp = 200;
        }
        else if (playerLevel < 153)
        {
            neededExp = 300;
        }
        else if (playerLevel < 204)
        {
            neededExp = 400;
        }
        else if (playerLevel < 255)
        {
            neededExp = 500;
        }
        else
        {
            neededExp = 600;
        }

        int currentExp = PlayerPrefs.GetInt("Exp", 0);
        if (currentExp >= neededExp)
        {
            PlayerPrefs.SetInt("Level", playerLevel + 1);
            PlayerPrefs.SetInt("Exp", currentExp - neededExp);
        }

        profile.UpdateTexts();
    }

    public void ClickXp()
    {
        if (canShowAd)
        {
            int xp = GetXp();
            if (xp == 0)
            {
                xpText.text = "More ads tomorrow";
                canShowAd = false;
            }
            else
            {
                ShowRewardedAd(xp);
            }
        }
    }
}