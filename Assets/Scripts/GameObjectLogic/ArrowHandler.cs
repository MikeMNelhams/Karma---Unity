using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler
{
    readonly GameObject _arrow;

    public ArrowHandler(GameObject currentPlayerArrow)
    {
        _arrow = currentPlayerArrow;
    }

    public void SetArrowVisibility(bool isVisible)
    {
        _arrow.SetActive(isVisible);
    } 

    public void MoveArrow(Vector3 playerPosition, Vector3 tablePosition)
    {
        Quaternion towardsTable = Quaternion.LookRotation(tablePosition - playerPosition);
        _arrow.transform.SetPositionAndRotation(playerPosition + new Vector3(0, 3.5f, 0), Quaternion.Euler(0, towardsTable.eulerAngles.y + 90, 0));
    }

    public void PointUp()
    {
        _arrow.transform.rotation = Quaternion.Euler(new Vector3(180, 90, 0));
    }

    public void PointDown()
    {
        _arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
    }
}
