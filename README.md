PROBLEM 1:
If the application is called from different Web browser windows/Browser tabs, on the same worstation/device, and user i slogger in with the same credentials on both windows, displayed patient information must be synchronized so that the same Dialog is displayed on page.

Solution:
For the first problem, searching some possible solutions online I found and used a combination of two approaches: 
  - The use of localStorage events
  - The use of a BroadcastChannel API

I chose to go with a hybrid of the two to keep maximum reliability since the first approach (localStorage events) can be helpfull for cross-tab persistence and the second approach (BroadCastChannel) is helpfull for real-time updates. I also used some Listeners to update other tabs that have subscribed to the event, to stay in sync.


Diagram:

sequenceDiagram
  participant Tab1
  participant LocalStorage
  participant BroadcastChannel
  participant Tab2

  Tab1->>LocalStorage: Set selectedPatientId=123
  LocalStorage->>Tab2: StorageEvent (sync)
  Tab1->>BroadcastChannel: Post message
  BroadcastChannel->>Tab2: Receive message



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
For this problem I thought about how to manage and implement a JWT-based SSO with a shared auth service.

All applications must trust the same auth mechanism and all user sessions must have a single source of truth.

To implement this I had to implement both a shared JWT secret and OAuth2, to have both a shared secter via JWT and a central session store via Redis.
I added an Auth Service to have a centralized login endpoint (JWT Issuer), I setup a cross-domain cookie by setting up a domain cookie readable by all subdomains and setup a Token verification system.

The token verification system checks the local JWT Token. If the token is expired or missing, it silently verify via the auth service and Redis tracks the active session for revocation.

This approach requires the use of HTTPS for cookies and tokens, to securely exchange secrets.


Diagram:

graph TD
  A[User Logs In] --> B[Auth Service]
  B --> C[Set .domain.com Cookie]
  C --> D[App1: Read Cookie/JWT]
  C --> E[App2: Read Cookie/JWT]
  C --> F[AppN: Read Cookie/JWT]
  D --> G[Access Granted]


Code example: 


	app.post('/login', (req, res) => {
 		// Validate credentials and set cross-domain cookie
	  	const token = jwt.sign({ userId }, SHARED_SECRET, { expiresIn: '1h' });
	  
	  	res.cookie('sso_token', token, {
	    		domain: '.yourdomain.com',
	    		httpOnly: true,
	    		secure: true,
	    		sameSite: 'lax'
	  	});
	  
	  	res.json({ token });
	});

	// for all N apps
	const verifySSO = async () => {
	  	try {
    			// 1. Check local token first
	    		const localToken = localStorage.getItem('token');
	    		if (localToken && jwt.verify(localToken, SHARED_SECRET)) return true;

     			 // 2. Check for SSO cookie
	    		const { data } = await axios.get('https://domain.com/verify', {
	     		 withCredentials: true
	   	 });
	    
	    	if (data.valid) {
	     		 localStorage.setItem('token', data.token);
	      		return true;
	    	}
	    		return false;
	  	} catch {
	    	return false;
	  	}
	};
