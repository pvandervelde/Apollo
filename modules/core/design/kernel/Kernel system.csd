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
      <Text>Could we do a fluent API for the core system? That will allow easy additions too (think extention methods)?</Text>
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
      <Location left="637" top="984" />
      <Size width="206" height="102" />
    </Entity>
    <Entity type="Comment">
      <Text>Forms the front end of the project sub-system</Text>
      <Location left="2112" top="754" />
      <Size width="162" height="72" />
    </Entity>
    <Entity type="Comment">
      <Text>All communication between services runs through the message pipeline</Text>
      <Location left="2082" top="428" />
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
      <Location left="2864" top="316" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>IMessage</Name>
      <Access>Public</Access>
      <Location left="2579" top="478" />
      <Size width="162" height="71" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Interface">
      <Name>ICommandMessage</Name>
      <Access>Public</Access>
      <Location left="2579" top="316" />
      <Size width="162" height="74" />
      <Collapsed>False</Collapsed>
    </Entity>
    <Entity type="Comment">
      <Text>Timeline service will have to track blocking events (e.g. removal of a plug-in, replacement of a plug-in etc.) because these events make it impossible to follow the timeline backwards or forwards</Text>
      <Location left="2545" top="1330" />
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
      <Location left="2864" top="1421" />
      <Size width="162" height="96" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>ProjectService</Name>
      <Access>Public</Access>
      <Location left="2082" top="877" />
      <Size width="162" height="92" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>TimelineService</Name>
      <Access>Public</Access>
      <Location left="2629" top="1190" />
      <Size width="162" height="85" />
      <Collapsed>False</Collapsed>
      <Modifier>Sealed</Modifier>
    </Entity>
    <Entity type="Class">
      <Name>MessagePipeline</Name>
      <Access>Public</Access>
      <Location left="2082" top="561" />
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
      <Name>ServiceProvider</Name>
      <Access>Public</Access>
      <Location left="563" top="465" />
      <Size width="253" height="129" />
      <Collapsed>False</Collapsed>
      <Modifier>None</Modifier>
    </Entity>
    <Entity type="Comment">
      <Text>How do we deal with data storage? Does that need to be a service too? Could be a smart way of handling storage because then we can abstract it. Allows file / database etc.</Text>
      <Location left="202" top="833" />
      <Size width="230" height="107" />
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
      <Location left="2247" top="1683" />
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
        <X>2198</X>
        <Y>1000</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2573</X>
        <Y>1218</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="19" second="21">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2839</X>
        <Y>1449</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2816</X>
        <Y>1238</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="19" second="22">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2895</X>
        <Y>1368</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2721</X>
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
        <X>2794</X>
        <Y>1465</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1899</X>
        <Y>1434</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="20" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2043</X>
        <Y>940</Y>
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
        <X>2828</X>
        <Y>1214</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2269</X>
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
        <X>2046</X>
        <Y>912</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2040</X>
        <Y>622</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Generalization" first="21" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2602</X>
        <Y>1245</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1980</X>
        <Y>1245</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="22" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2056</X>
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
        <X>1603</X>
        <Y>1302</Y>
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
        <X>1540</X>
        <Y>1182</Y>
      </BendPoint>
    </Relation>
    <Relation type="Generalization" first="25" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>963</X>
        <Y>587</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1591</X>
        <Y>1205</Y>
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
        <X>2000</X>
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
        <X>1934</X>
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
    <Relation type="Comment" first="28" second="26">
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
    <Relation type="Generalization" first="29" second="17">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2196</X>
        <Y>1728</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>1876</X>
        <Y>1358</Y>
      </BendPoint>
    </Relation>
    <Relation type="Association" first="19" second="29">
      <StartOrientation>Horizontal</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2799</X>
        <Y>1483</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2382</X>
        <Y>1647</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="20" second="29">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>2180</X>
        <Y>997</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2217</X>
        <Y>1707</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="23" second="29">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Horizontal</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2075</X>
        <Y>1751</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="24" second="29">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="True">
        <X>1232</X>
        <Y>1112</Y>
      </BendPoint>
      <BendPoint relativeToStartShape="False">
        <X>2298</X>
        <Y>1839</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
    <Relation type="Association" first="25" second="29">
      <StartOrientation>Vertical</StartOrientation>
      <EndOrientation>Vertical</EndOrientation>
      <BendPoint relativeToStartShape="False">
        <X>2332</X>
        <Y>1857</Y>
      </BendPoint>
      <Direction>Unidirectional</Direction>
      <IsAggregation>False</IsAggregation>
      <IsComposition>False</IsComposition>
    </Relation>
  </Relations>
</ClassProject>