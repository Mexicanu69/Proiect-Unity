using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OcclusionHider : MonoBehaviour
{
    public Transform target; //playerul                
    public float targetHeight = 1f;          //pivotul playerului

    public LayerMask obstacleLayers;     
    public float radius = 0.3f;              // grosimea razei (doar pentru SphereCastAll)
    public float castDistance = 100f;
    HashSet<Renderer> obiecteBlocate = new HashSet<Renderer>();
    Dictionary<Renderer, ShadowCastingMode> hidden = new Dictionary<Renderer, ShadowCastingMode>();
    List<Renderer> toRemove = new List<Renderer>();

    void LateUpdate()
    {
        //calculam pozitiile camera, target si vectorul de la camera la target
        Vector3 cameraPosition = transform.position * castDistance;               // camera la frame curent
        Vector3 targetPosition = target.position + Vector3.up * targetHeight;
        Vector3 dir = targetPosition - cameraPosition;

        Debug.DrawLine(cameraPosition, targetPosition, Color.red);

        //vedem care sunt obiectele lovite pe vectorul camera:player
        obiecteBlocate.Clear();
        RaycastHit[] hits = Physics.SphereCastAll(cameraPosition, radius, dir.normalized,
                                          dir.magnitude, obstacleLayers);
        //fiecare obiect are Renderer(tine de desenarea pe ecran: material, umbre, etc) si Collider (forma fizica)
        foreach(var hit in hits)
        {   
            //deci pentru fiecare obiect vom lua componenta Renderer a obiectului si o vom adauga in obiecte pe care le blocam la frame curent
            Renderer r = hit.collider.GetComponent<Renderer>();
            if(r != null && !r.transform.IsChildOf(target)) obiecteBlocate.Add(r);
        }

        //adaugam starea obiectului inainte de a fii hidden si apoi il facem invizibil
        foreach(var r in obiecteBlocate)
        {
            if (!hidden.ContainsKey(r))
            {
                hidden[r] = r.shadowCastingMode;
                r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        toRemove.Clear();
        //parcurgem lista de hidden si vedem daca obiectul blocat nu mai este blocat la frameul curent si il facem din nou vizibil
        foreach (var hiddenObject in hidden)
        {
            Renderer r = hiddenObject.Key;
            if (r == null) { toRemove.Add(r); continue; }          // obiectul a fost distrus

            //daca nu mai este in lista de block la frame curent inseamna ca nu mai trebuie sa fie hidden, asa ca ii dam restore
            if (!obiecteBlocate.Contains(r))
            {
                r.shadowCastingMode = hiddenObject.Value;                   
                toRemove.Add(r);
            }
        }

        //la final de script facem totul vizibil indiferent
        foreach (var r in toRemove)
            hidden.Remove(r);
    }
}