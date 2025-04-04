allprojects {
    repositories {
        google()
        mavenCentral()
    }
}

// Set a custom build directory for the root project
val newBuildDir = rootProject.layout.projectDirectory.dir("../../build")
rootProject.buildDir = newBuildDir.asFile

// Set custom build directories for subprojects
subprojects {
    buildDir = rootProject.buildDir.resolve(project.name)
}

// Clean task to delete the custom build directory
tasks.register<Delete>("clean") {
    delete(rootProject.buildDir)
}