using System;
using UnityEngine;

public class ByteBuffer
{
    byte[] buffer;
    int pointer = 0;

    public void Put(byte data)
    {
        buffer[pointer] = data;
        pointer++;
    }

    public void Put(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);

        buffer[pointer] = bytes[0];
        buffer[pointer + 1] = bytes[1];
        buffer[pointer + 2] = bytes[2];
        buffer[pointer + 3] = bytes[3];

        pointer += 4;
    }

    public void Put(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);

        buffer[pointer] = bytes[0];
        buffer[pointer + 1] = bytes[1];
        buffer[pointer + 2] = bytes[2];
        buffer[pointer + 3] = bytes[3];

        pointer += 4;
    }

    // Depends on the availability of Put(byte);, therefore not pure
    public void Put(string s)
    {
        if (s.Length > 255)
        {
            Debug.LogError("Perkele joni lyhyempiä stringejä");
        }
        else
        {
            char[] chars = s.ToCharArray();

            Put((byte)chars.Length);
			
			foreach(char c in chars)
            {
                Put((byte)c);
            }
        }
    }

    public byte GetByte()
    {
        byte b = buffer[pointer];
        pointer++;

        return b;
    }

    public int GetInt()
    {
        int val = BitConverter.ToInt32(buffer, pointer);
        pointer += 4;

        return val;
    }

    public float GetFloat()
    {
        float val = BitConverter.ToSingle(buffer, pointer);
        pointer += 4;

        return val;
    }

    // Depends on the availability of GetByte();, therefore not pure
    public string GetString()
    {
        byte len = GetByte();

        string temp = "";

        for (int i = 0; i < len; i++)
        {
            temp += (char)GetByte();
        }

        return temp;
    }


    public void SetPointer(int pointer)
    {
        this.pointer = pointer;
    }

    public int GetPointer()
    {
        return pointer;
    }


    public byte[] Get()
    {
        return buffer;
    }


    public ByteBuffer()
    {
        buffer = new byte[8192];
    }

    public ByteBuffer(byte[] buffer)
    {
        this.buffer = buffer;
    }

    public ByteBuffer Trim()
    {
        byte[] temp = new byte[pointer + 1];
        Array.Copy(buffer, 0, temp, 0, pointer);
        return new ByteBuffer(temp);
    }
}