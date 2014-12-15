using UnityEngine;
using System;
using System.Collections;

public class FHMainMenuManager : MonoBehaviour {
    public GameObject status;

	void Start()
    {
        SetStatus(false);

		// Send install source
		SendInstallSource();

        // Check for show rating
        CheckShowRating();

        // Check for need to migrate
        if (FHPlayerProfile.instance.targetApp != "")
        {
            GUIMessageDialog.Show((res) =>
                {
                    FHUtils.OpenAppStore(FHPlayerProfile.instance.targetApp);
                    return false;
                },
                FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_ACCOUNT_CLOUD_STORED),
                FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
                FH.MessageBox.MessageBoxButtons.OK
            );

            return;
        }
        
        // Check for update version
        CheckUpdateVersion();
    }

    void CheckShowRating()
    {
#if UNITY_ANDROID
//        if (FHPlayerProfile.instance.lastTimeShowRating != -1 && DateTime.Now.Ticks - FHPlayerProfile.instance.lastTimeShowRating > TimeSpan.TicksPerDay)
//        {
//            GuiManager.ShowPanel(GuiManager.instance.guiRating);
//            FHPlayerProfile.instance.lastTimeShowRating = DateTime.Now.Ticks;
//        }
#endif
    }

    void CheckUpdateVersion()
    {
        string appID = FHSystem.instance.GetFullAppID();
        if(true)
        return;
        FHHttpClient.GetClientUpdate(appID, (res, json) =>
        {
            if (res == FHResultCode.OK)
            {
                string version = json["version"];
                bool forceUpdate = json["forceUpdate"].AsInt == 1;
                string updateUrl = json["updateUrl"];
                bool isNeedMigrating = json["needMigrating"].AsBool;
                int enableLocalPayment = json["enableLocalPayment"].AsInt;

                if (updateUrl == null || updateUrl == "")
                    updateUrl = FHSystem.instance.appIdentifier;

                FHSystem.instance.enableLocalPayment = enableLocalPayment == 1;

                if (isNeedMigrating)
                {
                    ShowUpdateVersionWithMigrating(updateUrl);
                    return;
                }

                string currVer = ((TextAsset)Resources.Load(appID, typeof(TextAsset))).text;
                if (version == null || currVer == null)
                    return;

                if (int.Parse(version.Replace(".", "")) > int.Parse(currVer.Replace(".", "")))
                    ShowUpdateVersion(updateUrl, forceUpdate, version);
                else
                    CheckRestore();
            }
        });
    }

    void ShowUpdateVersion(string updateUrl, bool forceUpdate, string version)
    {
        GUIMessageDialog.Show((r) =>
            {
                if (r == FH.MessageBox.DialogResult.Ok)
                    FHUtils.OpenAppStore(updateUrl);

                return !forceUpdate;
            },
            string.Format(FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE), version),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OK
        );
    }

    void ShowUpdateVersionWithMigrating(string updateUrl)
    {
        GUIMessageDialog.Show((r) =>
            {
                if (r == FH.MessageBox.DialogResult.Ok)
                {
                    if (FHPlayerProfile.instance.dataRestored)
                    {
                        FHUtils.OpenAppStore(updateUrl);
                        return false;
                    }
                    else
                    {
                        SetStatus(true);
                        FHSystem.instance.StorePlayerData(updateUrl, OnStorePlayerDataSuccess, OnStorePlayerDataFail);
                        return true;
                    }
                }
                else
                    return true;
            },
            FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_NEED_MIGRATING),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OKCancel
        );
    }

    void OnStorePlayerDataSuccess(string targetApp)
    {
        SetStatus(false);

        FHGoldHudPanel.instance.UpdateGold();

        GUIMessageDialog.Show((r) =>
            {
                FHUtils.OpenAppStore(targetApp);

                return false;
            },
            FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_STORE_SUCCESS),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OK
        );
    }

    void OnStorePlayerDataFail()
    {
        SetStatus(false);

        GUIMessageDialog.Show(
            null,
            FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_STORE_FAIL),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OK
        );
    }

    void CheckRestore()
    {
        FHHttpClient.CheckProfile((code, json) =>
        {
            if (code == FHResultCode.OK)
            {
                GUIMessageDialog.Show((r) =>
                    {
                        if (r == FH.MessageBox.DialogResult.Ok)
                        {
                            SetStatus(true);
                            FHSystem.instance.RestorePlayerData(OnRestorePlayerDataSuccess, OnRestorePlayerDataFail);
                            return true;
                        }
                        else
                            return true;
                    },
                    FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_RESTORE_DATA),
                    FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
                    FH.MessageBox.MessageBoxButtons.OKCancel
                );
            }
        });
    }

    void OnRestorePlayerDataSuccess()
    {
        SetStatus(false);

        FHGoldHudPanel.instance.UpdateGold();

        GUIMessageDialog.Show(
            null,
            FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_RESTORE_SUCCESS),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OK
        );
    }

    void OnRestorePlayerDataFail()
    {
        SetStatus(false);

        GUIMessageDialog.Show(
            null,
            FHLocalization.instance.GetString(FHStringConst.VERSION_UPDATE_RESTORE_FAIL),
            FHLocalization.instance.GetString(FHStringConst.REQUIRE_UPDATE_TITLE),
            FH.MessageBox.MessageBoxButtons.OK
        );
    }

    void SetStatus(bool isWaiting)
    {
        status.SetActiveRecursively(isWaiting);
    }

	void SendInstallSource()
	{
		if (FHSystem.instance.installSource != "" && FHHttpClient.isInternetReachable && !FHPlayerProfile.instance.installSourceSent)
		{
			Debug.LogError("[Pre-install] Send for " + FHSystem.instance.installSource);
//			GoogleAnalyticsBinding.SendEvent("Pre-install", FHSystem.instance.installSource, "installed", 1);
			FHPlayerProfile.instance.installSourceSent = true;
		}
	}
}