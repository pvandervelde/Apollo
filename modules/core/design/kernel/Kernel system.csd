<ClassProject>
  <Language>CSharp</Language>
  <Entities>
    <Entity type="Class">
      <Name>BootStrapper</Name>
      <Access>Public</Access>
      <Location left="418" top="276" />
      <Size width="162" height="94" />
      <Collapsed>False</Collapsed>
      <Member type="Method">public void Start()</Member>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Interface">
      <Name>IAppDomainBuilder</Name>
      <Access>Public</Access>
      <Location left="826" top="316" />
      <Size width="162" height="65" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Services can also specify LoadBefore information. This indicates before which services they need to be loaded. 

Note that most system services are always loaded in a specific order / time. e.g. logging, messages, license</Text>
      <Location left="724" top="1546" />
      <Size width="190" height="76" />
    </Entity>
    <Entity type="Comment">
      <Text>Loads the User Interface service. All other services will be started from the dependencies for the UI service</Text>
      <Location left="395" top="132" />
      <Size width="204" height="69" />
    </Entity>
    <Entity type="Interface">
      <Name>INeedStartup</Name>
      <Access>Public</Access>
      <Location left="563" top="1190" />
      <Size width="354" height="112" />
      <Collapsed>False</Collapsed>
      <Member type="Event">event EventHandler&lt;StartupProgressEventArgs&gt; StartupProgress</Member>
      <Member type="Method">void Start()</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Needs way to do:
- error stuff
- async stuff
- Indicate complete</Text>
      <Location left="642" top="984" />
      <Size width="206" height="102" />
    </Entity>
    <Entity type="Comment">
      <Text>Forms the front end of the project sub-system</Text>
      <Location left="2516" top="692" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>All communication between services runs through the message pipeline</Text>
      <Location left="2486" top="428" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Interface">
      <Name>IOwnServices</Name>
      <Access>Public</Access>
      <Location left="563" top="742" />
      <Size width="236" height="175" />
      <Collapsed>False</Collapsed>
      <Member type="Method">void Install(IService service)</Member>
      <Member type="Method">void Uninstall(IService service)</Member>
      <Member type="Method">void Start(IService service)</Member>
      <Member type="Method">void Start(Type serviceType)</Member>
      <Member type="Method">void Stop(IService service)</Member>
      <Member type="Method">void Restart(IService service)</Member>
    </Entity>
    <Entity type="Comment">
      <Text>Services have dependencies which have to be loaded before the service can be loaded.

A service is not normally running. Once a message for the service arrives it will be loaded and the message can be processed. 
Some services will be started on application start up.</Text>
      <Location left="710" top="1351" />
      <Size width="187" height="160" />
    </Entity>
    <Entity type="Interface">
      <Name>ICommand</Name>
      <Access>Public</Access>
      <Location left="3033" top="222" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IMessage</Name>
      <Access>Public</Access>
      <Location left="2743" top="384" />
      <Size width="162" height="71" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandMessage</Name>
      <Access>Public</Access>
      <Location left="2743" top="222" />
      <Size width="162" height="74" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Timeline service will have to track blocking events (e.g. removal of a plug-in, replacement of a plug-in etc.) because these events make it impossible to follow the timeline backwards or forwards</Text>
      <Location left="2977" top="1478" />
      <Size width="243" height="92" />
    </Entity>
    <Entity type="Comment">
      <Text>Allows the core system to easily interact with the different services on an equal level. Mainly use to get to the message pipeline.</Text>
      <Location left="1034" top="368" />
      <Size width="209" height="83" />
    </Entity>
    <Entity type="Comment">
      <Text>Need some way of allowing additional services to be started without needing to rely on the dependencies coming from the UI service</Text>
      <Location left="642" top="132" />
      <Size width="256" height="66" />
    </Entity>
    <Entity type="Comment">
      <Text>This also needs to do license verification?</Text>
      <Location left="202" top="287" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Class">
      <Name>KernelService</Name>
      <Access>Public</Access>
      <Location left="1628" top="1160" />
      <Size width="302" height="153" />
      <Collapsed>False</Collapsed>
      <Member type="Property">public abstract ServiceType ServiceType { get; }</Member>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>MarshalByRef</Name>
      <Access>Public</Access>
      <Location left="1370" top="1050" />
      <Size width="162" height="82" />
      <Collapsed>False</Collapsed>
      <Modifier>Abstract</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>UserInterfaceService</Name>
      <Access>Public</Access>
      <Location left="3296" top="1569" />
      <Size width="162" height="96" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ProjectService</Name>
      <Access>Public</Access>
      <Location left="2486" top="815" />
      <Size width="162" height="92" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>TimelineService</Name>
      <Access>Public</Access>
      <Location left="3061" top="1338" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>MessagePipeline</Name>
      <Access>Public</Access>
      <Location left="2486" top="561" />
      <Size width="162" height="84" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>LogSink</Name>
      <Access>Public</Access>
      <Location left="1239" top="1421" />
      <Size width="162" height="90" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>PluginRepository</Name>
      <Access>Public</Access>
      <Location left="1146" top="908" />
      <Size width="162" height="122" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>CoreProxy</Name>
      <Access>Public</Access>
      <Location left="999" top="531" />
      <Size width="162" height="112" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>Kernel</Name>
      <Access>Public</Access>
      <Location left="563" top="465" />
      <Size width="253" height="129" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>Does the kernel do the exception handling? Or is there a service for that?

Probably no service, however all AppDomains / Threads / Services will have to be individually hardend to ensure that we can actually pass the errors onto the UI and the logger.</Text>
      <Location left="176" top="490" />
      <Size width="258" height="124" />
    </Entity>
    <Entity type="Class">
      <Name>PersistenceService</Name>
      <Access>Public</Access>
      <Location left="2651" top="1683" />
      <Size width="162" height="94" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Interface">
      <Name>IProcessMessages</Name>
      <Access>Public</Access>
      <Location left="1691" top="240" />
      <Size width="393" height="129" />
      <Collapsed>False</Collapsed>
      <Member type="Method">void ProcessMessage(KernelMessage message)</Member>
      <Member type="Method">void ProcessMessages(params KernelMessage[] messages)</Member>
      <Member type="Method">void ProcessMessages(IEnumerable&lt;KernelMessage&gt; messages)</Member>
      <Member type="Method">void ProcessMessages(IEnumerator&lt;KernelMessages&gt; messages)</Member>
    </Entity>
    <Entity type="Interface">
      <Name>IHaveServiceDependencies</Name>
      <Access>Public</Access>
      <Location left="1341" top="287" />
      <Size width="238" height="83" />
      <Collapsed>False</Collapsed>
      <Member type="Method">IEnumerator&lt;Type&gt; Dependencies()</Member>
    </Entity>
    <Entity type="Class">
      <Name>LicenseService</Name>
      <Access>Public</Access>
      <Location left="2013" top="1006" />
      <Size width="162" height="105" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>how to deal with configurations? Where do we store the default config file. And how do we write the config files.</Text>
      <Location left="2747" top="1995" />
      <Size width="202" height="86" />
    </Entity>
    <Entity type="Comment">
      <Text>Could we do something with hashkeys to verify that the answers coming from the license service are correct?
Obvious problem is that all data runs on the users machine so they have full access to the system.</Text>
      <Location left="1987" top="1127" />
      <Size width="280" height="103" />
    </Entity>
    <Entity type="Comment">
      <Text>What happens to messages that can't be delivered? e.g. due to a service dying etc.?</Text>
      <Location left="3004" top="384" />
      <Size width="217" height="62" />
    </Entity>
    <Entity type="Comment">
      <Text>Services should be able to register scanners / filters etc.?</Text>
      <Location left="1414" top="908" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>Can we do the license system so that we store some code / IL in an encrypted part of the application. The license key is then used to unencrypt that code. The code can then be 'compiled' and passed on to the different services. That way we can extract different parts of the project / UI / persistence service into an encrypted part of the application (private key encrypted, decrypt with the public key).</Text>
      <Location left="1987" top="1245" />
      <Size width="340" height="118" />
    </Entity>
  </Entities>
  <Relations>
    <Relation type="Comment" first="3" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="5" second="4">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>740</X>
        <Y>1150</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="12" second="11">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="12" second="10">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="15" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>565</X>
        <Y>251</Y>
      </BendPoint>
    </Relation>
    <Relation type="Realization" first="17" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1589</X>
        <Y>1243</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>946</X>
        <Y>1243</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="17" second="18">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1567</X>
        <Y>1100</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="9" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>945</X>
        <Y>1452</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1034</X>
        <Y>1256</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="2" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>941</X>
        <Y>1570</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1104</X>
        <Y>1274</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="13" second="21">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="6" second="20">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="20" second="21">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2602</X>
        <Y>938</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>3005</X>
        <Y>1366</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="19" second="21">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3271</X>
        <Y>1597</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>3248</X>
        <Y>1386</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="19" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3327</X>
        <Y>1516</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>3125</X>
        <Y>615</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="19" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3222</X>
        <Y>1633</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1842</X>
        <Y>1432</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="20" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2450</X>
        <Y>870</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1831</X>
        <Y>1118</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="21" second="22">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3260</X>
        <Y>1362</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2673</X>
        <Y>630</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="20" second="22">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2450</X>
        <Y>850</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2444</X>
        <Y>622</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="21" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3034</X>
        <Y>1393</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1884</X>
        <Y>1374</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="22" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2460</X>
        <Y>611</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1793</X>
        <Y>1135</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="23" second="17">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>1599</X>
        <Y>1291</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="24" second="17">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1257</X>
        <Y>1115</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1603</X>
        <Y>1204</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="25" second="17">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1110</X>
        <Y>698</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1594</X>
        <Y>1223</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="24" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1225</X>
        <Y>850</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2404</X>
        <Y>593</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="25" second="22">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1209</X>
        <Y>579</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2338</X>
        <Y>579</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="7" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Comment" first="14" second="25">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Realization" first="26" second="8">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Realization" first="26" second="4">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>529</X>
        <Y>559</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>521</X>
        <Y>1245</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="26" second="25">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>883</X>
        <Y>561</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>965</X>
        <Y>561</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Association" first="0" second="26">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>511</X>
        <Y>399</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>532</X>
        <Y>531</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="16" second="0">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
    </Relation>
    <Relation type="Association" first="26" second="1">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>788</X>
        <Y>351</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>True</IsComposition>
    </Relation>
    <Relation type="Comment" first="27" second="26">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>464</X>
        <Y>548</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>538</X>
        <Y>548</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="28" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2600</X>
        <Y>1728</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1812</X>
        <Y>1356</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="19" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>3362</X>
        <Y>1708</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="20" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2584</X>
        <Y>935</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2621</X>
        <Y>1707</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="23" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2479</X>
        <Y>1751</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="24" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1232</X>
        <Y>1112</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2702</X>
        <Y>1839</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="25" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2736</X>
        <Y>1857</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="31" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1966</X>
        <Y>1048</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1886</X>
        <Y>1135</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="20" second="31">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2442</X>
        <Y>890</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="28" second="31">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2626</X>
        <Y>1716</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2482</X>
        <Y>1040</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="32" second="28">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2776</X>
        <Y>1802</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="33" second="31">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2294</X>
        <Y>1180</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2200</X>
        <Y>1097</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="22" second="11">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2735</X>
        <Y>590</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Comment" first="34" second="11">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2979</X>
        <Y>419</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2930</X>
        <Y>419</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="35" second="24">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1492</X>
        <Y>852</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1268</X>
        <Y>883</Y>
      </BendPoint>
    </Relation>
    <Relation type="Comment" first="36" second="31">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2352</X>
        <Y>1296</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2318</X>
        <Y>1072</Y>
      </BendPoint>
    </Relation>
  </Relations>
</ClassProject>