// Author: alex@rozgo.com
// License: enjoy

using UnityEngine;
using System.Collections;

public class Finger {
   public iPhoneTouch touch;
   public bool moved = false;
   public ArrayList colliders = new ArrayList();
}

public class FingerManager : MonoBehaviour {
   
   public ArrayList fingers = new ArrayList();
   
   void Update () {
      RaycastHit[] hits;
      foreach (iPhoneTouch evt in iPhoneInput.touches) {
         if (evt.phase==iPhoneTouchPhase.Began) {
            Finger finger = new Finger();
            finger.touch = evt;
            Ray ray = Camera.main.ScreenPointToRay(evt.position);
            hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits) {
               finger.colliders.Add(hit.collider);
               GameObject to = hit.collider.gameObject;
               to.SendMessage("FingerBegin",evt,SendMessageOptions.DontRequireReceiver);
            }
            fingers.Add(finger);
         }
         else if (evt.phase==iPhoneTouchPhase.Moved) {
            for (int i=0;i<fingers.Count;++i) {
               Finger finger = (Finger)fingers[i];
               if (finger.touch.fingerId==evt.fingerId) {
                  finger.moved = true;
				  //added new raycast check
				  if (finger.colliders.Count == 0)
				  {
				    	Ray ray = Camera.main.ScreenPointToRay(evt.position);
						hits = Physics.RaycastAll(ray);
						foreach (RaycastHit hit in hits) {
							finger.colliders.Add(hit.collider);
						}
				  }	
                  foreach (Collider collider in finger.colliders) {
                     if (collider==null) 
						{
							continue;
						}
                     GameObject to = collider.gameObject;
                     	to.SendMessage("FingerMove",evt,SendMessageOptions.DontRequireReceiver);
                  }

               }
            }
         }
         else if (evt.phase==iPhoneTouchPhase.Ended || evt.phase==iPhoneTouchPhase.Canceled) {
            Ray ray = Camera.main.ScreenPointToRay(evt.position);
            hits = Physics.RaycastAll(ray);
            for (int i=0;i<fingers.Count;) {
               Finger finger = (Finger)fingers[i];
               if (finger.touch.fingerId==evt.fingerId) {
                  foreach (Collider collider in finger.colliders) {
                     if (collider==null) {continue;}
                     bool canceled = true;
                     foreach (RaycastHit hit in hits) {
                        if (hit.collider==collider) {
                           canceled = false;
                           GameObject to = collider.gameObject;
                           to.SendMessage("FingerEnd",evt,SendMessageOptions.DontRequireReceiver);
                        }
                     }
                     if (canceled) {
                        GameObject to = collider.gameObject;
                        to.SendMessage("FingerCancel",evt,SendMessageOptions.DontRequireReceiver);
                     }
                  }
                  fingers[i] = fingers[fingers.Count-1];
                  fingers.RemoveAt(fingers.Count-1);
               }
               else {
                  ++i;
               }
            }
         }
      }
   }
}