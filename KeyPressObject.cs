using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace BobTheBuilder
{
    public class KeyPressObject : MonoBehaviour
    {
        public static Action OnUpdate;

        public KeyPressObject(IntPtr ptr)
            : base(ptr)
        {
        }

        public KeyPressObject()
            : base(ClassInjector.DerivedConstructorPointer<KeyPressObject>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }

        private void Update()
        {
            if (OnUpdate == null)
            {
                return;
            }

            OnUpdate?.Invoke();
        }
    }
}
