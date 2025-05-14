using UnityEngine;

public class MyLinkedList<T>
{
    private int count; 
    public int Count => count;
    public Node<T> head;
   
    public MyLinkedList()
    {

        head = null;
        count = 0;
    }
    public void Add(T data)
    {
        Node<T> newNode = new Node<T>(data);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node<T> current = head;
            while (current.next != null)
            {
                current = current.next;
            }
            current.next = newNode;
        }
        count++;
    }

    public T GetAt(int index) { 
    
        if (index < 0 || index >= count)
            throw new System.IndexOutOfRangeException();
        
            Node <T>   current = head;
            for ( int i = 0; i < index; i++)
            {
                current = current.next;
            }
            
        return current.data;
    }
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= count)
            throw new System.IndexOutOfRangeException();

        if (index == 0)
        {
            head = head.next;
        }
        else
        {
            Node<T> current = head;
            for (int i = 0; i < index - 1; i++)
                current = current.next;

            current.next = current.next.next;
        }
        count--;
    }

    public void Print()
    {
        Node<T> current = head;
        while (current != null)
        {
            UnityEngine.Debug.Log(current.data);
            current = current.next;
        }
    }

}
