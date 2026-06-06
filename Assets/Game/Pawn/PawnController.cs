using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class PawnController : MonoBehaviour
{
    private List<IPawnCommand> commands = new List<IPawnCommand>();
    private bool IsCyclic = false;
    private int index = 0;
    private bool IsRunning = false;
    
    private Pawn pawn;
    private Coroutine _executionRoutine;

    private 

    void Awake() => pawn = GetComponent<Pawn>();

    public void IssueCommands(List<IPawnCommand> newCommands)
    {
        StopCurrentExecution();
        commands = new List<IPawnCommand>(newCommands);
        StartExecution();
    }

    public void AddCommand(IPawnCommand command)
    {
        commands.Add(command);
        
        if (!IsRunning)
        {
            StartExecution();
        }
    }

    private void StartExecution()
    {
        IsRunning = true;
        _executionRoutine = StartCoroutine(CommandLoop());
    }

    private void StopCurrentExecution()
    {
        if (_executionRoutine != null) StopCoroutine(_executionRoutine);
        IsRunning = false;
        index = 0;
    }

    private IEnumerator CommandLoop()
    {
        while (IsRunning)
        {
            if (index >= commands.Count)
            {
                if (IsCyclic && commands.Count > 0)
                {
                    index = 0;
                }
                else
                {
                    IsRunning = false;
                    yield break;
                }
            }

            IPawnCommand currentCommand = commands[index];
            bool commandFinished = false;
            bool commandSuccess = false;

            currentCommand.Execute(pawn.gameObject, (agent, success) => 
            {
                commandSuccess = success;
                commandFinished = true;
            });

            yield return new WaitUntil(() => commandFinished);

            if (commandSuccess)
            {
                index++;
            }
            else
            {
                IsRunning = false;
            }
        }
    }

    public void ClearCommands()
    {
        StopCurrentExecution();
        commands.Clear();
    }

    public void CyclicCommands(bool state) => IsCyclic = state;
}
