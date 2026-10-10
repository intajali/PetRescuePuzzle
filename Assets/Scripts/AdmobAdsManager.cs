using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using EasyUI.Toast;

public class AdmobAdsManager : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;

    public static AdmobAdsManager instance;

    [Header("Banner Ad ID")]
    public string AndroidBannerID;
    public string IosBannerID;

    [Header("Interstitial Ad ID")]
    public string AndroidInterstitialID;
    public string IosInterstitialID;


    [Header("Rewarded Ad ID")]
    public string AndroidRewardedID;
    public string IosRewardedID;


    [Header("Rewarded Interstitial Ad ID")]
    public string AndroidRewardedInterID;
    public string IosRewardedInterID;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize the Google Mobile Ads SDK.
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            print("Ads Initialised !!");

        });

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }

        //this.RequestBanner();
        this.RequestInterstitial();
        this.RequestRewardedAd();


        DontDestroyOnLoad(this);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidBannerID;
#elif UNITY_IPHONE
        string adUnitId = IosBannerID;
#else
            string adUnitId = "unexpected_platform";
#endif

        /*        // Create a 320x50 banner at the bottom of the screen.
                this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

                //create a banner
                CreateBannerView();

                // Create an empty ad request.
                AdRequest request = new AdRequest.Builder().Build();

                // Load the banner with the request.
                this.bannerView.LoadAd(request);
        */
        //create a banner
        CreateBannerView(adUnitId);

        //listen to banner events
        ListenToBannerEvents();

        //load the banner
        if (bannerView == null)
        {
            CreateBannerView(adUnitId);
        }

        var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        print("Loading banner Ad !!");
        bannerView.LoadAd(adRequest);//show the banner on the screen

    }
    void CreateBannerView(string bannerId)
    {

        if (bannerView != null)
        {
            DestroyBannerAd();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(bannerId, adaptiveSize, AdPosition.Bottom);
        
    }

    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Banner view paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyBannerAd()
    {

        if (bannerView != null)
        {
            print("Destroying banner Ad");
            bannerView.Destroy();
            bannerView = null;
        }
    }

    //INTERSTITIAL AD
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidInterstitialID;
#elif UNITY_IPHONE
        string adUnitId = IosInterstitialID;
#else
        string adUnitId = "unexpected_platform";
#endif


        // Clean up the old ad before loading a new one.
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitial = ad;
            });
    }



    //Where you need to show Interstitial Ad Just Add this Line there
    // AdmobAdsManager.instance.ShowInterstitial();
    public void ShowInterstitial()
    {
        if (interstitial != null && interstitial.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitial.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            RequestInterstitial();
        }
        RegisterReloadHandler(interstitial);
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };


    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            RequestInterstitial();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            RequestInterstitial();
        };
    }


    //REWARDED AD

    public void RequestRewardedAd()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = AndroidRewardedID;
#elif UNITY_IPHONE
        adUnitId = IosRewardedID;
#else
            adUnitId = "unexpected_platform";
#endif


        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });

    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
                "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            RegisterEventHandlers(rewardedAd);
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }

        else
        {
            RequestRewardedAd();
        }
    }

    public void ShowRewardedAd(UnityAction<bool> onComplete)
    {
        const string rewardMsg =
                "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            RegisterEventHandlers(rewardedAd, onComplete);
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                Toast.Show("Rewarded ad Completed", 2f, Color.green, ToastPosition.BottomCenter);
                //onComplete(true);
            });
           
        }
        else
        {
            //onComplete(false);
            RequestRewardedAd();
            Toast.Show("Rewarded ad not available", 2f, Color.red, ToastPosition.BottomCenter);
        }
    }

    //REWARDED AD EVENTS
    private void RegisterEventHandlers(RewardedAd ad, UnityAction<bool> onComplete)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            onComplete(true);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            RequestRewardedAd();
            onComplete(true);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }


    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            RequestRewardedAd();
           
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
    //REWARDED AD EVENTS ENDED



    public void RequestRewardedInterAd()
    {

        string adUnitId;
#if UNITY_ANDROID
        adUnitId = AndroidRewardedInterID;
#elif UNITY_IPHONE
        adUnitId = IosRewardedInterID;
#else
            adUnitId = "unexpected_platform";
#endif

        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        //adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedInterstitialAd.Load(adUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedInterstitialAd = ad;
            });

    }

    public void ShowRewardedInterstitialAd()
    {
        Debug.Log("ShowRewardedInterstitialAd");
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }

        else
        {
            RequestRewardedInterAd();
        }
        RegisterEventHandlers(rewardedInterstitialAd);

    }
    public void ShowRewardedInterstitialAd(UnityAction<bool> onComplete)
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }

        RegisterEventHandlers(rewardedInterstitialAd, onComplete);

    }

    private void RegisterEventHandlers(RewardedInterstitialAd ad, UnityAction<bool> onComplete)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            RequestRewardedInterAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);
        };
    }

    private void RegisterEventHandlers(RewardedInterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            RequestRewardedInterAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open " +
                           "full screen content with error : " + error);
        };
    }

}
