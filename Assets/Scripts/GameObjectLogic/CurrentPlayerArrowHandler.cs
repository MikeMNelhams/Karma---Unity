using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayerArrowHandler
{
    readonly GameObject _currentPlayerArrow;

    public CurrentPlayerArrowHandler(GameObject currentPlayerArrow)
    {
        _currentPlayerArrow = currentPlayerArrow;
    }

    public void SetArrowVisibility(bool isVisible)
    {
        _currentPlayerArrow.SetActive(isVisible);
    } 

    public void MoveArrow(Vector3 playerPosition, Vector3 tablePosition)
    {
        Quaternion towardsTable = Quaternion.LookRotation(tablePosition - playerPosition);
        _currentPlayerArrow.transform.SetPositionAndRotation(playerPosition + new Vector3(0, -1.5f, 0), Quaternion.Euler(0, towardsTable.eulerAngles.y, 90));
    }
}
