using Assets.Scripts.Sounds;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
    public ItemModel model;
    public bool CanTake = true;
    private bool isDo = false;
    PlayerControl pc;
    private AudioSource source;
    private Collider i_collider;
    public void Start()
    {
        source = this.AddComponent<AudioSource>();
        i_collider = this.GetComponent<Collider>();
        source.playOnAwake = false;
        source.volume = 0.2f;
        source.priority = 256;
    }
    public void Get(PlayerControl p)
    {
        if (CanTake)
        {
            pc = p;
            i_collider.enabled = false;
            switch (model.Type)
            {
                case ItemModel.TType.Coin: pc.Coins += (int)model.Value; break;
                case ItemModel.TType.Oxygen:{
                        pc.Oxygen += model.Value;
                        if (pc.Oxygen > 100)
                            pc.Oxygen = 100;}; break;
                case ItemModel.TType.Shield:
                    {
                        SoundConroller.PlaySouund("take_sound");
                        pc.isShield = true;
                        pc.ShieldMenu = pc.GameUI.AddExtraItem((int)ItemModel.TType.Shield, null, model.Duration, (s, i) =>
                        {
                            Debug.Log("Done " + i.id);
                            pc.isShield = false;
                        });
                    }
                    break;
                case ItemModel.TType.Magnit:
                    {
                        SoundConroller.PlaySouund("take_sound");
                        pc.isMagnit = true;
                        pc.GameUI.AddExtraItem((int)ItemModel.TType.Magnit, null, model.Duration, (s, i) =>
                        {
                            Debug.Log("Done " + i.id);

                            pc.isMagnit = false;
                        });
                    }
                    break;
                case ItemModel.TType.TimeSmoosh:
                    Time.timeScale = model.Value;
                    SoundConroller.PlaySouund("take_sound");
                    pc.GameUI.AddExtraItem((int)ItemModel.TType.Magnit, null, model.Duration, (s, i) =>
                    {
                        Debug.Log("Done " + i.id);
                        Time.timeScale = 1;
                    });
                    break;
            }
            isDo = true;
            StartCoroutine(GetAnination());
            source.PlayOneShot(model.Audio);
        }
    }
    private void Update()
    {
        if (isDo)
        {
            transform.position = Vector3.Lerp(transform.position,
                pc.transform.position + Vector3.up * 5, Time.deltaTime * 2f);
            if (!gameObject.activeInHierarchy) isDo = false;
        }
    }
    private IEnumerator GetAnination()
    {
        GameObject er_temp = Instantiate(model.Effect, transform.position+Vector3.up*2, Quaternion.identity);
        for (float i = 1; i >= 0; i-=0.05f)
        {
            this.gameObject.transform.localScale = new Vector3(i,i,i);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
    
}
