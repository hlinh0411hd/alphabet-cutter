using System;
using UnityEngine;

public class Trie : MonoBehaviour {
    public Node root;

	public Trie()
	{
        root = null;
	}

    public void insert(Node root, char[] word, int index) {
        if (root == null) {
            root = new Node(word[index]);
        }
        if (word[index] < root.data) {
            insert(root.left, word, index);
        }
        if (word[index] > root.data) {
            insert(root.right, word, index);
        }
        if (word[index] == root.data) {
            if (index != word.Length - 1) {
                insert(root, word, index + 1);
            } else {
                root.end = 1;
            }
        }
    }

    public bool isTrueWord(Node root, char[] word, int index) {
        if (root == null) {
            return false;
        }
        if (word[index] < root.data) {
            return isTrueWord(root.left, word, index);
        }
        if (word[index] > root.data) {
            return isTrueWord(root.right, word, index);
        }
        if (word[index] == root.data) {
            if (index != word.Length - 1) {
                isTrueWord(root.mid, word, index + 1);
            } else {
                if(root.end == 1) {
                    return true;
                }
            }
        }
        return false;
    }
    
}
