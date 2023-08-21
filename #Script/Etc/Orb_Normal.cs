using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Normal : Orb
{
	public override void Reward()
	{
		int count_Coin = Random.Range(5, 10);
		int count_Item = Random.Range(2, 4);
		Canvas_Player.instance.Coin(count_Coin);
		DropTable dropTable = Canvas_Player.instance.GetOrbTable(this);
		if (dropTable == null)
		{
			print("드롭테이블이 없습니다!");
			return;
		}
		foreach (var item in dropTable.GetItem(count_Item))
		{
			Canvas_Player.instance.AddItem(item);
		}
		
	}
}
