using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CSGOStat
{
    public class BootStrapper : BootstrapperBase
    {
        private CompositionContainer _container;
        public BootStrapper()
        {
            LogManager.GetLog = type => new Debugger(type);
            Initialize();
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            //Tells Caliburn Micro(CM) to display the root view of the specified type. You can have a base view, and then other views up the visual tree....
            //or something like that. Google it i dont remember.
            DisplayRootViewFor<IShell>();
        }
        #region MEF
        /* Managed Extensibility Framework
         * A lot of this is just copy paste code, from http://caliburnmicro.com/documentation/bootstrapper
         * This is used to make whatever we store in our CompositionContainer available across the application.
         * In this example we have put the windowmanager and the evenAggregator in it.
         * The WindowManager manages creation and showing of windows/dialogs/stuff like that. 
         * The EventAggregator is a service that provides us with the ability to publish objects from one entity to another, in a loose fashion. http://caliburnmicro.com/documentation/event-aggregator
         * Once the configure part of this is setup, you can go to your respective classes, and mark them with the [Export] attribute, and then if you for example
         * wanted to get a local instance of the windowmanager, mark the constructor with the [ImportingConstructor], like this:
         *  [ImportingConstructor]
         *  public AppViewModel(IWindowManager windowManager)
         * which will make the framework put in the applications instance of the windowmanager, for use in whatever you want to.
         */
        protected override void Configure()
        {
            _container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            //Collects values/stuff that has to go in our container
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);
            _container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
        #endregion
    }
}
