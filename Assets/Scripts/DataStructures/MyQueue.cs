using System.Threading;
using UnityEngine;

public class MyQueue<T>
{
    public Node<T> front;
    public Node<T> rear;
    private int count;
    public int Count => count;
    public void Enqueue(T data)
    {
        Node<T> newNode = new Node<T>(data);
        if (rear==null)
        {
            front = newNode;
            rear = newNode;

        }
        else 
        {
            rear.next = newNode;
            rear=newNode;
  
        }
        count++;

    }
    public T Dequeue()
    {

        if (IsEmpty())
        {

            throw new System.InvalidOperationException("Stack is empty.");
        }
        T data = front.data;
        front = front.next;
        if(front==null)
        {
            rear =null;
        }
        count--;
        return data;
        
    }
    public bool IsEmpty() 
    {
    return count==0;
    }
   
}
