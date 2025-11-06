using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionControlManager : Singleton<VersionControlManager>
{
    const string VersionKey = "Version";

    protected override void Initialize()
    {
        //만약 현재 최신버전 업데이트가 아니라면 파일 제거
        if (!PlayerPrefs.HasKey(VersionKey) || PlayerPrefs.GetString(VersionKey) != Application.version)
        {
            SaveManager.Instance.DestroyAllSlots();
        }

        PlayerPrefs.SetString(VersionKey, Application.version);
        Destroy(gameObject);
    }
}
