using System;
using System.Reflection;
using System.Windows.Input;
using Mastoom.Shared.Models.Common;
using Xamarin.Forms;

namespace Mastoom.Behaviors
{
    public class EventToCommand : Behavior<VisualElement>
    {
        public static readonly BindableProperty EventNameProperty =
            BindableProperty.Create(
                "EventName",
                typeof(string),
                typeof(EventToCommand),
                "",
                propertyChanged: OnEventNameChanged);

        public string EventName
        {
            get {
                var obj = GetValue(EventNameProperty);
                return obj?.ToString() ?? "null-da"; 
            }
            set { 
                SetValue(EventNameProperty, value); 
            }
        }

        public static readonly BindableProperty HelperProperty =
            BindableProperty.Create(
            "Helper",
            typeof(OAuthModel),
            typeof(EventToCommand),
            (OAuthModel)null,
            propertyChanged: OnHelperChanged);

        public OAuthModel Helper
        {
            get
            {
                var obj = GetValue(HelperProperty);
                return (OAuthModel)obj;
            }
            set
            {
                 SetValue(HelperProperty, value);
            }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                "Command",
                typeof(ICommand),
                typeof(EventToCommand),
                null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty ConverterProperty =
            BindableProperty.Create(
                "Converter",
                typeof(IValueConverter),
                typeof(EventToCommand),
                null);

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        private Delegate eventHandler;

        private VisualElement associatedObject;

        private static void OnEventNameChanged(
            BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = bindable as EventToCommand;
            if (behavior.associatedObject == null)
            {
                return;
            }

            var oldEventName = oldValue as string;
            var newEventName = newValue as string;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }


        private static void OnHelperChanged(
            BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = bindable as EventToCommand;
            if (behavior.associatedObject == null)
            {
                return;
            }

            var oldEventName = oldValue;
            var newEventName = newValue;
        }




        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            associatedObject = bindable;

            if (bindable.BindingContext != null)
            {
                BindingContext = bindable.BindingContext;
            }
            bindable.BindingContextChanged += OnBindingContextChanged;

            RegisterEvent(EventName);
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            DeregisterEvent(EventName);

            bindable.BindingContextChanged -= OnBindingContextChanged;

            associatedObject = null;

            base.OnDetachingFrom(bindable);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            BindingContext = associatedObject.BindingContext;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        private void RegisterEvent(string name)
        {
            var eventInfo = associatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                return;
            }

            var methodInfo = typeof(EventToCommand).GetTypeInfo().GetDeclaredMethod("OnEvent");
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(associatedObject, eventHandler);
        }

        private void DeregisterEvent(string name)
        {
            if (eventHandler == null)
            {
                return;
            }

            var eventInfo = associatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                return;
            }

            eventInfo.RemoveEventHandler(associatedObject, eventHandler);
            eventHandler = null;
        }

        private void OnEvent(object sender, object eventArgs)
        {
            if (Command != null)
            {
                object param = eventArgs;

                if (Converter != null)
                {
                    param = Converter.Convert(eventArgs, typeof(object), null, null);
                }

                if (Command.CanExecute(param))
                {
                    Command.Execute(param);
                }
            }
        }
    }
}
