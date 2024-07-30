using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionNumberDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _versionTMP;

    private void Awake()
    {
#if UNITY_EDITOR
        _versionTMP.gameObject.SetActive(true);
        _versionTMP.text = "Version: " + Application.version;
#else
        Destroy(_versionTMP);
#endif
    }
}
