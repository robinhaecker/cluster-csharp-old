﻿using System;
using Cluster.GameMechanics.Content;
using Cluster.GameMechanics.Interface.Commons;
using Cluster.GameMechanics.Universe;
using Cluster.GameMechanics.Universe.CelestialBodies;
using Cluster.GameMechanics.Universe.LivingThings;
using Cluster.Mathematics;

namespace Cluster.GameMechanics.Interface
{
    public class PlanetSelection : ISelection
    {
        private Planet planet;
        private int pickedIndex;

        private Panel panel;

        public PlanetSelection()
        {
            panel = new Panel(10, GameWindow.active.height - 110, 10, 2);
        }

        public bool selectByPick(float x, float y)
        {
            foreach (Planet pl in Planet.planets)
            {
                double delx = x - pl.x;
                double dely = y - pl.y;
                double dist = delx * delx + dely * dely;
                if (dist < pl.size * pl.size * 625.0)
                {
                    planet = pl;
                    pickedIndex = -1;

                    if (Space.zoom > 0.3 && dist > pl.size * pl.size * 225.0)
                    {
                        double alpha = Math.Atan2(dely, delx) / (2.0 * Math.PI);
                        pickedIndex = (int) Math.Floor(((alpha + 1.0) % 1.0) * pl.size);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool selectByBox(Vec4 box)
        {
            planet = null;
            pickedIndex = -1;
            return false;
        }

        public void setActive(bool active)
        {
            if (active)
            {
                panel.enable();
            }
            else
            {
                panel.disable();
            }
        }

        public bool isMouseOver()
        {
            return panel.isMouseOver();
        }

        public string getToolTipText()
        {
            var elementAtMousePosition = panel.getElementAtMousePosition();
            return (elementAtMousePosition as IToolTip)?.getInfoText();
        }

        public Vec2 getCenterOfMass()
        {
            return new Vec2(planet.x, planet.y);
        }

        public void updateGui()
        {
            panel.enable();
            panel.clear();
            if (pickedIndex > -1)
            {
                if (planet.infra[pickedIndex] != null)
                {
                    var mainButton = new MeshButton(planet.infra[pickedIndex].blueprint.shape,
                        Planet.terraImage[(int)planet.terra[pickedIndex]],
                        0, 0,
                        Commons.Properties.BUTTON_SIZE_LARGE);
                    mainButton.setInfoText(planet.infra[pickedIndex].getInfoText());
                    panel.addLargeElement(mainButton);
                }
                else
                {
                    var mainButton = new MeshButton(Planet.terraImage[(int)planet.terra[pickedIndex]], 
                        null,
                        0, 0,
                        Commons.Properties.BUTTON_SIZE_LARGE);
                    mainButton.setInfoText(planet.getTerrainType(pickedIndex));
                    panel.addLargeElement(mainButton);
                    foreach (Blueprint blueprint in Blueprint.getAllBuildableOn(planet, pickedIndex))
                    {
                        var button = new MeshButton(blueprint.shape);
                        button.setInfoText(blueprint.getInfoText());
                        button.onLeftClick(() => blueprint.isBuildable(planet, pickedIndex)
                                                && Civilisation.getPlayer().ressources >= blueprint.getCost()
                            ? planet.build(pickedIndex, blueprint, Civilisation.getPlayer())
                            : null);
                        panel.addElement(button);
                    }
                }
            }

            panel.updateState();
        }

        public void renderGui()
        {
            panel?.render();
        }
    }
}