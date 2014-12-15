using UnityEngine;
using System.Collections;
using System;
namespace FH.MessageBox
{
    public enum DialogResult
    {
        None,
        Ok,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public enum MessageBoxButtons
    {
        OK=0,
        OKCancel,
        AbortRetryIgnore,
        YesNoCancel,
        YesNo,
        RetryCancel
    }

    public enum MessageBoxDefaultButton
    {
        Button1,
        Button2,
        Button3
    }

    public enum MessageBoxIcon
    {
        None,
        Hand,
        Exclamation,
        Asterisk,
        Stop,
        Error,
        Warning,
        Information
    }

    public delegate bool MessageCallback(DialogResult result);

    public class MessageItem
    {
        // Fields
        public MessageBoxButtons buttons;
        public MessageCallback callback;
        public string caption;
        public MessageBoxDefaultButton defaultButton;
        public string message;

        // Methods
        public MessageItem()
        {
            this.message = string.Empty;
            this.caption = string.Empty;
            this.buttons = MessageBoxButtons.OK;
            this.defaultButton = MessageBoxDefaultButton.Button1;
        }

        public MessageItem(MessageCallback call, string content, string cap, MessageBoxButtons btns, MessageBoxDefaultButton defaultBtn)
        {
            this.message = content;
            this.caption = cap;
            this.buttons = btns;
            this.defaultButton = defaultBtn;
        }

    }
}

 

