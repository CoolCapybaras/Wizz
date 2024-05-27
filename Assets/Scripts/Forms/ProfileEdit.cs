using Net.Packets.Serverbound;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEdit : MonoBehaviour, IForm
{
    public static ProfileEdit Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Serializable]
    public struct Form
    {
        public RawImage avatar;
        public TMP_InputField nicknameInputField;
    }

    public Form form;

    private Texture2D avatarTexture;
    public void InitializeForm()
    {
        form.nicknameInputField.text = LocalClient.instance.Name;
        avatarTexture = LocalClient.instance.Image.GetTexture();
        form.avatar.texture = avatarTexture;
    }

    public void OnSavePressed()
    {
        if (!UpdateProfilePacket.NameRegex.IsMatch(form.nicknameInputField.text))
        {
            OverlayManager.Instance.ShowInfo("Неверное имя", InfoType.Error);
            return;
        }

        SendChangesToServer();
    }

    public void OnCancelChangesPressed()
    {
        InitializeForm();
    }

    public void OnAvatarChangePressed()
    {
        Helpers.GetTexture(SetAvatar);
    }

    public void SetAvatar(Texture2D texture)
    {
        avatarTexture = texture;
        form.avatar.texture = texture;
    }

    private void SendChangesToServer()
    {
        LocalClient.instance.SendPacket(new UpdateProfilePacket()
        {
            Name = form.nicknameInputField.text,
            Image = new ByteImage(avatarTexture.EncodeToJPG()),
            Type = 0
        });

        LocalClient.instance.Image = new ByteImage(avatarTexture.EncodeToJPG());
        LocalClient.instance.Name = form.nicknameInputField.text;
    }
}
