using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

public class MyStack <T>
{
    public Node<T> top;
    private int count;
    public int Count => count;
    public void Push(T data)
    {
        Node<T> newNode = new Node<T>(data);
       newNode.next= top;
        top = newNode;
        count++;
    }
    public T Pop() 
    {
        if (IsEmpty())
        {

            throw new System.InvalidOperationException("Stack is empty.");
        }
        T data= top.data;
        top=top.next;
        count--;
        return data;
    }
    public bool IsEmpty()
    {
        return count == 0;
    }
    public T Peek()
    {
        if (IsEmpty())
            throw new System.InvalidOperationException("Stack is empty.");
        return top.data;
    }
    public void AddRange(IEnumerable<T> Collection)
    {
        foreach (var item in Collection)
        {
            Push(item);
        }
    }
}
