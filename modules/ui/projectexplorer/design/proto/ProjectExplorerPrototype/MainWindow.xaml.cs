using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Annotations;
using SpiderTreeControl.Diagram;
using BarberBornander.UI.Physics;
using BarberBornander.UI.Physics.SpringRenderers;

namespace ProjectExplorerPrototype
{
    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Data
        
        Particle anchor;
        private Particle[] particlesLatched;
        private Spring[] anchorSpringsLatched;
        private Spring[] particleSpringsLatched;
        private Spring springLastToFirstLatched;

        #endregion

        /// <summary>
        /// Constructor for the main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object obj, EventArgs e)
        {
            InitialiseSystem();
        }

        private void InitialiseSystem()
        {
            try
            {
                //create the Customers particles
                InitialiseCustomers();
                //initialise other Canvas objects
                pcGraph.OwnerWindow = this;
                this.PreviewMouseDown += new MouseButtonEventHandler(
                    pcGraph.ParticleCanvas_PreviewMouseDown);
                this.PreviewMouseMove += new MouseEventHandler(
                    pcGraph.ParticleCanvas_PreviewMouseMove);
                this.PreviewMouseUp += new MouseButtonEventHandler(
                    pcGraph.ParticleCanvas_PreviewMouseUp);
                pcGraph.RenderParticleSystem = true;
                pcGraph.StartSimulation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        

        /// <summary>
        /// Create the Customer Particles
        /// </summary>
        private void InitialiseCustomers()
        {
            try
            {
                Particle[] particles = new Particle[5];
                int startPos = 100;

                //setup Usercontrol particles
                for (int i = 0; i < particles.Count(); i++)
                {
                    if (i == 0)
                    {
                        particles[i] = new Particle(double.PositiveInfinity,
                            new Vector(startPos, startPos), true);
                    }
                    else
                    {
                        startPos += 50;
                        particles[i] = new Particle(double.PositiveInfinity,
                            new Vector(startPos, startPos), true);
                    }
                    particles[i].Control = getAnchorButton();
                    pcGraph.ParticleSystem.Particles.Add(particles[i]);
                }

                //setup anchor
                anchor = new Particle(float.PositiveInfinity,
                    new Vector((double)(
                        this.pcGraph.ActualWidth / 2), 40), true);
                anchor.Control = getAnchorButton();
                pcGraph.ParticleSystem.Particles.Add(anchor);
                //now generate the pyhsics for these new Particles
                GeneratePhysicsForParticles(particles, anchor, false, 840.0f, 260.0f, 60.0f);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBoxHelper.ShowMessageBox(ex.Message, "An error occurred");
            }
        }

        /// <summary>
        /// Creates and adds new Particles to the contained
        /// ParticleCanvas
        /// </summary>
        /// <param name="particles">The array of particles</param>
        /// <param name="anchor">The anchor Particle</param>
        /// <param name="createNewobjects">True if we are to remember
        /// the objects that were created by calling this method</param>
        /// <param name="restFirstToEnd">rest fisrt to end Particles</param>
        /// <param name="restLengthToAnchor">rest length</param>
        /// <param name="restLengthToEachOther">length to each other</param>
        private void GeneratePhysicsForParticles(Particle[] particles,
            Particle anchor, bool createNewobjects, float restFirstToEnd,
            float restLengthToAnchor, float restLengthToEachOther)
        {
            try
            {
                //springs
                Spring[] anchorSprings = new Spring[particles.Count()];
                Spring[] particleSprings = new Spring[particles.Count() - 1];
                Spring springLastToFirst;

                //pens
                Pen transparentPen = new Pen(Brushes.Transparent, 1.0);
                Pen whiteSmokeDashPen = new Pen(Brushes.Gainsboro, 2.0);
                whiteSmokeDashPen.DashStyle = DashStyles.Dash;
                Pen whiteSmokeSolidPen = new Pen(Brushes.Gainsboro, 4.0);

                //renderers
                RubberBand transparentBandRenderer =
                    new RubberBand(transparentPen);
                RubberBand whiteSmokeDashBandRenderer =
                    new RubberBand(whiteSmokeDashPen);
                RubberBand whiteSmokeSolidRenderer =
                    new RubberBand(whiteSmokeSolidPen);

                //setup anchorSprings
                for (int i = 0; i < particles.Length; i++)
                {
                    //260.0f
                    anchorSprings[i] = new Spring(anchor, particles[i],
                        restLengthToAnchor, 0.5f, 5.0f);
                    anchorSprings[i].Renderer = whiteSmokeSolidRenderer;
                    pcGraph.ParticleSystem.Springs.Add(anchorSprings[i]);
                }

                //setup particleSprings
                for (int i = 0; i < particleSprings.Length; i++)
                {
                    //60.0f
                    particleSprings[i] = new Spring(particles[i],
                        particles[i + 1], restLengthToEachOther, 1.0f, 10.0f);
                    particleSprings[i].Renderer = whiteSmokeDashBandRenderer;
                    pcGraph.ParticleSystem.Springs.Add(particleSprings[i]);
                }

                //setup first to last very strong spring
                springLastToFirst = new Spring(particles[0],
                    particles[particles.Length - 1], restFirstToEnd, 3.5f, 2.0f);
                springLastToFirst.Renderer = transparentBandRenderer;
                pcGraph.ParticleSystem.Springs.Add(springLastToFirst);

                if (createNewobjects)
                {
                    particlesLatched = particles;
                    anchorSpringsLatched = anchorSprings;
                    particleSpringsLatched = particleSprings;
                    springLastToFirstLatched = springLastToFirst;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                MessageBoxHelper.ShowMessageBox(ex.Message, "An error occurred");
            }
        }

        /// <summary>
        /// Creates a special Button which will be
        /// used at the anchor for all other Particles
        /// within the 
        /// ParticleCanvas
        /// </summary>
        /// <returns>The anchor Button</returns>
        private Button getAnchorButton()
        {
            BitmapImage bitmap = new BitmapImage(
                new Uri("../Images/anchor.png", UriKind.Relative));
            Image image = new Image();
            image.Source = bitmap;
            image.Width = 55;
            image.Height = 55;
            Button btn = new Button();
            btn.Content = image;
            btn.Width = double.NaN;
            btn.Height = double.NaN;
            btn.SetValue(Canvas.ZIndexProperty, -1);
            ControlTemplate anchorButtonTemplate =
                this.TryFindResource("anchorButtonTemplate") as ControlTemplate;
            if (anchorButtonTemplate != null)
                btn.Template = anchorButtonTemplate;
            pcGraph.Children.Add(btn);
            btn.PreviewMouseUp += new MouseButtonEventHandler(
                pcGraph.ParticleCanvas_PreviewMouseUp);
            btn.PreviewMouseMove += new MouseEventHandler(
                pcGraph.ParticleCanvas_PreviewMouseMove);
            btn.PreviewMouseDown += new MouseButtonEventHandler(
                pcGraph.ParticleCanvas_PreviewMouseDown);
            return btn;
        }

        /// <summary>
        /// On Window SizeChanged, update the conatined Particle positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect bounds = new Rect(this.pcGraph.RenderSize);

            if (anchor != null)
                anchor.SetPosition(new Vector((double)
                    (pcGraph.ActualWidth / 2), 40), bounds);
        }
    }
}
