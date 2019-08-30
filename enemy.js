 // manage the enemy movement
update: function(dt) {
this.now = new Date().getTime();
    if (this.alive) {
      if (this.walkLeft && this.pos.x <= this.startX) {
      this.walkLeft = false;
          this.touchSound = false; // here is where I change it to false
          this.body.setVelocity( Math.floor((Math.random()*7)+1), Math.floor((Math.random()*6)+1));
	  }
    } else if (!this.walkLeft && this.pos.x >= this.endX) {
        this.walkLeft = true;
        this.touchSound = false; // here too
        this.body.setVelocity( Math.floor((Math.random()*6)+1), Math.floor((Math.random()*6)+1));
    }
      var buffer = this.walkLeft ? -32:32;
      if((Math.round(this.now/1000)%3 === 0) && ((this.now - this.lastShot) >= 1000)){
        this.lastShot = this.now;
        var bullet = me.pool.pull("EnemyBullet", this.pos.x+buffer, this.pos.y, {
		image: 'bullet',
		spritewidth: 24,
		spriteheight: 24,
		width: 24,
		height: 24
		}, [this.prevMove, downOn, leftOn, rightOn]);
		me.game.world.addChild(bullet, this.z);
    }
        
    if(this.pos.x - game.data.playerX <= 96 &&
        ( this.pos.y - game.data.playerY <= 32 ||
          this.pos.y - game.data.playerY >=32)){
          this.moveDir = 1;
		}
		else{
            this.moveDir = 3;
        }
		
    // make it walk
    if(this.walkLeft){
        this.prevMove = 3;
        this.body.vel.x +=-this.body.accel.x * me.timer.tick;
        
		if(!this.renderable.isCurrentAnimation("run_left")){
        this.renderable.setCurrentAnimation("run_left");
		}
	}
    else if(!this.walkLeft){
           this.prevMove = 1;
            this.body.vel.x +=this.body.accel.x * me.timer.tick;
        if(!this.renderable.isCurrentAnimation("run_right")){
            this.renderable.setCurrentAnimation("run_right");
        }
        
     
    } 
	
	else {
      this.body.vel.x = 0;
    }
           
    // update the body movement
    this.body.update(dt);
     
    // handle collisions against other shapes
    me.collision.check(this);
      
       
    // return true if we moved or if the renderable was updated
    return (this._super(me.Entity, 'update', [dt]) || this.body.vel.x !== 0 || this.body.vel.y !== 0);
  }
   
  /**
   * colision handler
   * (called when colliding with other objects)
   */
  onCollision : function (response, other) {
    if(response.b.body.collisonType === me.collision.types.PROJECTILE_OBJECT) return true;
    if (response.b.body.collisionType !== me.collision.types.WORLD_SHAPE) {
      // res.y >0 means touched by something on the bottom
      // which mean at top position for this one

		if (this.alive && ((response.overlapV.y >= 0)  || (response.overlapV.x >= -5)) && response.b.body.collisionType === me.collision.types.PLAYER_OBJECT ) {

		// this.renderable.flicker(750);
        //here i removed flickering and added audio stuff
			var choose = Math.floor((Math.random()*3)+1);
			if(this.touchSound == false){
				switch(choose){
					case 1: me.audio.play("criminalscum", .5);
						this.touchSound = true;
						break;
            
					case 2: me.audio.play("stoprest", .5); this.touchSound = true;break;
                
					case 3: me.audio.play("freezescumbag", .5);this.touchSound = true; break;
                  
					default: me.audio.play("stoprest");this.touchSound = true; break;
				}
			}
          
		}
		if ((response.overlapV.x >= -5)&& response.b.body.collisionType !== me.collision.types.PLAYER_OBJECT){
			this.walkLeft = this.walkLeft ? false: true;
			this.body.setVelocity( Math.floor((Math.random()*6)+1), Math.floor((Math.random()*6)+1));
		}
    return false;
    }
    if ((response.overlapV.x >= -25)&& response.b.body.collisionType !== me.collision.types.PLAYER_OBJECT){
        this.walkLeft = this.walkLeft ? false: true;
        this.body.setVelocity( Math.floor((Math.random()*6)+1), Math.floor((Math.random()*6)+1));
    }
	
    // Make all other objects solid
    return true;
  }