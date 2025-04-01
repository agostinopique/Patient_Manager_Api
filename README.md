PROBLEM 1:
If the application is called from different Web browser windows/Browser tabs, on the same worstation/device, and user i slogger in with the same credentials on both windows, displayed patient information must be synchronized so that the same Dialog is displayed on page.

Solution:
For the first problem, searching some possible solutions online I found and used a combination of two approaches: 
  - The use of localStorage events
  - The use of a BroadcastChannel API

I chose to go with a hybrid of the two to keep maximum reliability since the first approach (localStorage events) can be helpfull for cross-tab persistence and the second approach (BroadCastChannel) is helpfull for real-time updates. I also used some Listeners to update other tabs that have subscribed to the event, to stay in sync.


Diagram:
[Tab 1] --> BroadcastChannel --> [Tab 2]
   ↑                               ↑
LocalStorage Event ←------→ LocalStorage Event



Code example, implemented in the Patient_Manager_Frontend project:

In a new SyncContext.tsx:

	import { useState, useEffect } from 'react';

	const useCrossTabSync = () => {
  	const [selectedPatientId, setSelectedPatientId] = useState<number | null>(null);
  
 	 useEffect(() => {
    const channel = new BroadcastChannel('patient_view_channel');
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === 'selectedPatient') {
        setSelectedPatientId(JSON.parse(e.newValue || 'null'));
      }
    };

    channel.addEventListener('message', (e) => {
      setSelectedPatientId(e.data.patientId);
    });

    window.addEventListener('storage', handleStorageChange);

    return () => {
      channel.close();
      window.removeEventListener('storage', handleStorageChange);
    };
 	 }, []);

  	const syncPatientView = (patientId: number | null) => {
    localStorage.setItem('selectedPatient', JSON.stringify(patientId));
    new BroadcastChannel('patient_view_channel').postMessage({
      type: 'PATIENT_VIEW',
      patientId
    });
  	};

 	 return { selectedPatientId, syncPatientView };
	};

In my HomePageComponent:

	const { selectedPatientId, syncPatientView } = useCrossTabSync();

	const handleOpenDialog = (patient: Patient) => {
 	 setSelectedPatientId(patient.id);
 	 syncPatientView(patient.id); // This will sync across tabs
	};




PROBLEM 2:
Problem 2: 
(Not strictly related to the above application)
Scenario: we have N web applications that can be installed on different servers. Once the user is logged into one of these applications, the other applications must recognize user login so that he won't need to insert credentials again.
