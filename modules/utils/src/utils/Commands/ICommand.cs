//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Passing the data in via a constructor or setter works, but requires the creator of the command to 
 * know the data the command needs...
 * The "context" idea is really good, and I was worked on (an internal) framework that leveraged it a 
 * while back.
 * If you set up your controller (UI components that interact with the user, CLI interpreting user 
 * commands, servlet interpreting incoming parameters and session data, etc) to provide named access 
 * to the available data, commands can directly ask for the data they want.
 * I really like the separation a setup like this allows. Think about layering as follows:
 * 
 * User Interface (GUI controls, CLI, etc)
 *     |
 * [syncs with/gets data]
 *     V
 * Controller / Presentation Model
 *     |                    ^
 * [executes]               |
 *     V                    |
 * Commands --------> [gets data by name]
 *     |
 * [updates]
 *     V
 * Domain Model
 * 
 * If you do this "right", the same commands and presentation model can be used with any type of user 
 * interface. Taking this a step further, the "controller" in the above is pretty generic. The UI 
 * controls only need to know the name of the command they'll invoke -- they (or the controller) don't 
 * need to have any knowledge of how to create that command or what data that command needs. That's the 
 * real advantage here.
 * 
 * For example, you could hold the name of the command to execute in a Map. Whenever the component 
 * is "triggered" (usually an actionPerformed), the controller looks up the command name, instantiates 
 * it, calls execute, and pushes it on the undo stack (if you use one).
 * 
 * 
 * Stack overflow: 
 * http://stackoverflow.com/questions/104918/command-pattern-how-to-pass-parameters-to-a-command/136565#136565
 */

namespace Apollo.Utils.Commands
{
    /// <summary>
    /// Defines the interface for a command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID for the command.</value>
        CommandId Id 
        {
            get;
        }

        /// <summary>
        /// Invokes the current command with the specified context as input.
        /// </summary>
        /// <param name="context">The context for the command.</param>
        void Invoke(ICommandContext context);
    }
}
