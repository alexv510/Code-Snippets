void Update () {

		//check if player is dead
		if(!isDead){

			//get camera rotation from mouse
			x += Input.GetAxis ("Mouse X") * xSpeed * distance * 0.0125f;
			if(y >= -90){
				y -= Input.GetAxis ("Mouse Y") * ySpeed * distance * 0.0025f;
			}else{
				y = -89.5f;
			}

			if(y <= 90){
				y -= Input.GetAxis ("Mouse Y") * ySpeed * distance * 0.0025f;
			}else{
				y = 89.5f;
			}

			//create and set rotation for camera target
			Quaternion rotation = Quaternion.Euler (y, x, 0);
			CameraTarg.rotation = rotation;



			//get speed modifier from abilities
			abilitySpeed = GetComponent<Abilities> ().abilitySpeedVal;
			//set initial player velocity 
			vel = Vector3.forward * Time.deltaTime * playerSpeed * abilitySpeed;
			
			
			//get velocity modifiers and direction from key movement
			bubbles.Pause();
			if (Input.GetKey ("w") && !Input.GetKey ("s")) { //move forwards
				ctRot = CameraTarg.transform.rotation;
				transform.rotation = ctRot; //set player direction/rotation
				transform.forward *= -1f;
				if(Time.deltaTime % 5 == 0) {
					bubbles.transform.position = transform.position;
					bubbles.Play();
				}

				if (acc < accMax) { //creates fluid acceleration/decceleration 
					acc += accCount;
					accCount += 0.005f;
					if (acc > accMax) {
						acc = accMax;
					}
				}
				if(Input.GetKey(KeyCode.LeftShift)){ //set boost
					vel *= -1f;
					transform.forward *= -1f;
					travelSpeed = 2f;
					tentaclesForward = false;
				}else{
					travelSpeed = 1f;
					tentaclesForward = true;
				}

			}else if(Input.GetKey ("s") && !Input.GetKey ("w")){ //move backwards
				ctRot = CameraTarg.transform.rotation;
				transform.rotation = ctRot; //set player direction/rotation
				transform.forward *= -1f;
				tentaclesForward = true;
				if (acc > deccMax){ //creates fluid acceleration/decceleration 
					acc -= deccCount;
					deccCount += 0.005f;
					if (acc < deccMax){
						acc = deccMax;
					}
				}
			}else{
				accCount = 0.025f;
				deccCount = 0.025f;

				//decrease speed to 0 naturally
				if((acc >= 0)){
					acc -= coast;

					if(coast > 0.01){
						coast -= 0.01f;
					}

					if(acc < 0){
						acc = 0;
						coast = 0.01f;
					}

				} else{
					acc += coastD;

					if(coastD > 0.01){
						coastD -= 0.01f;
					}

					if(acc > 0){
						acc = 0;
						coastD = 0.01f;
					}
				}

				if(!tentaclesForward)
				{
					vel *= -1;
				}
			}
			


			//slow player if carrying object
			if(GetComponent<PickupObject>().carrying){
				carryingSpeed = 0.5f;
			}else{
				carryingSpeed = 1f;
			}

			//change players position after true velocity is set
			transform.Translate (-vel * acc * inkAccBoost * carryingSpeed * travelSpeed);		

			//elevation control
			if (Input.GetKey ("r")){ //Move straight up
				transform.Translate (Vector3.up * Time.deltaTime * 8f);
			}
			if (Input.GetKey ("f")) //Move straight down
			{
				transform.Translate (Vector3.down * Time.deltaTime * 8f);
			}
		}

	}

	//restrict angle to be within 360 degrees
	public static float ClampAngle(float angle, float min, float max){
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	//Controls the ink movement, like moving forward or backwards depending on acceleration.
	public IEnumerator inkJump(){
		if(acc == 0 && inkJumpedBack == false){
			acc = -1;
			inkAccBoost = 5f;
			yield return StartCoroutine (waitThisLong(0.5f));
			acc = 0f;
			inkAccBoost = 1f;
			inkJumpedBack = true;
		}else if(inkJumpedBack != true){
			inkAccBoost = 5f;
			yield return StartCoroutine (waitThisLong(0.5f));
			inkAccBoost = 1f;
		}
		inkJumpedBack = false;
	}