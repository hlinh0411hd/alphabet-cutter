using System;
using UnityEngine;

public class Node : MonoBehaviour {
    public char data;
    public Node left, mid, right;
    public int end;
	public Node(char c)
	{
        data = c;
        left = mid = right = null;
        end = 0;
	}
}
